using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Sockets;
using System.Net.Security;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Jannesen.Library.Tasks;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public sealed class SMPPConnection: IDisposable
    {
        public  delegate    void        DelegateStateChanged(ConnectionState prevState, ConnectionState newState);
        public  delegate    Task        DelegateDeliverSm(SMPPDeliverSm message);

        private const   int EnquirePoll         = 60;

        private sealed class ActiveRequest
        {
            public      SMPPMessage                         Message         { get; set; }
            public      Task                                SendTask        { get; set; }
            public      int                                 TimeoutTicks    { get; set; }
            public      TaskCompletionSource<SMPPMessage>   TaskCompletion  { get; }

            public                                          ActiveRequest(SMPPMessage message)
            {
                Message        = message;
                TimeoutTicks   = 15;
                TaskCompletion = new TaskCompletionSource<SMPPMessage>();
            }
        }

        private sealed class ActiveRequestList
        {
            public              int                 ActiveCount
            {
                get {
                    lock(_list) {
                        return _list.Count;
                    }
                }
            }
            public              bool                isBusy
            {
                get {
                    lock(_list) {
                        return _list.Count > 0;
                    }
                }
            }

            private readonly    List<ActiveRequest> _list;


            public              ActiveRequest       AddMessage(SMPPMessage message)
            {
                var sendingMessage = new ActiveRequest(message);

                lock(_list) {
                    _list.Add(sendingMessage);
                }

                return sendingMessage;
            }

            public                                  ActiveRequestList()
            {
                _list = new List<ActiveRequest>();
            }

            public              void                CompleteError(ActiveRequest sendingMessage, Exception err)
            {
                lock(_list) {
                    _list.Remove(sendingMessage);
                }

                sendingMessage.TaskCompletion.TrySetException(err);
            }
            public              void                CompleteResp(SMPPMessage message)
            {
                ActiveRequest sendingMessage;

                lock(_list) {
                    var idx = _findMessage(message);
                    sendingMessage = _list[idx];
                    _list.RemoveAt(idx);
                }

                sendingMessage.TaskCompletion.SetResult(message);
            }
            public              void                TimeoutPoll()
            {
                List<ActiveRequest> timeoutMessages = null;

                lock(_list) {
                    for (var i = 0 ; i < _list.Count ; ++i) {
                        if (--(_list[i].TimeoutTicks) <= 0) {
                            if (timeoutMessages == null) {
                                timeoutMessages = new List<ActiveRequest>();
                            }

                            timeoutMessages.Add(_list[i]);
                        }
                    }
                }

                if (timeoutMessages != null) {
                    foreach(var m in timeoutMessages)
                        CompleteError(m, new TimeoutException("Timeout"));
                }
            }
            public              void                ConnectionDown()
            {
                lock(_list) {
                    while (_list.Count > 0) {
                        var msg = _list[0];
                        _list.RemoveAt(0);

                        CompleteError(msg, new SMPPException("Connection down."));
                    }
                }
            }

            private             int                 _findMessage(SMPPMessage message)
            {
                for (var i = 0 ; i < _list.Count ; ++i) {
                    if (_list[i].Message.Sequence == message.Sequence &&
                        (_list[i].Message.Command  == (message.Command & ~CommandSet.Response) || message.Command == CommandSet.GenericNack))
                        return i;
                }

                throw new SMPPException("Received response for a unknown message.");
            }
        }

        public              string                  Hostname        { get; set; }
        public              int                     Port            { get; set; }
        public              bool                    Tls             { get; set; }
        public              SMPPBind                Bind            { get; }
        public              string                  Url
        {
            get {
                return (Tls ? "smpps://" : "smpp://") + Hostname + ":" + Port.ToString(CultureInfo.InvariantCulture);
            }
            set {
                var url = new Uri(value);

                switch(url.Scheme) {
                case "smpp":    Tls = false;    Port = 2775;    break;
                case "smpps":   Tls = true;     Port = 2776;    break;
                default:        throw new ArgumentException("Invalid connection schema.");
                }

                Hostname = url.Host;

                if (url.Port > 0)
                    Port = url.Port;
            }
        }
        public              ConnectionState         State           { get { return _state;                      } }
        public              int                     ActiveRequests  { get { return _activeRequests.ActiveCount; } }
        public              Exception               Error           { get { return _error;                      } }
        public              DelegateStateChanged    OnStateChange;
        public              DelegateDeliverSm       OnDeliverSm;

        private volatile    ConnectionState         _state;
        private             UInt32                  _sequence;
        private             TcpClient               _tcpClient;
        private             Stream                  _stream;
        private             TaskLock                _sendLock;
        private readonly    ActiveRequestList       _activeRequests;
        private             int                     _enquirePoll;
        private             Exception               _error;

        private             bool                    _isRunning
        {
            get {
                lock(this) {
                    switch(_state) {
                    case ConnectionState.StreamConnected:
                    case ConnectionState.Binding:
                    case ConnectionState.Connected:
                    case ConnectionState.Unbinding:
                        return true;

                    default:
                        return false;
                    }
                }
            }
        }

        public                                      SMPPConnection()
        {
            Bind = new SMPPBind()
                            {
                                BindingType         = BindingType.Transceiver,
                                InterfaceVersion    = SmppVersionType.Version3_4
                            };
            _state          = ConnectionState.Closed;
            _activeRequests = new ActiveRequestList();
        }
        public              void                    Dispose()
        {
            Close();
        }

        public  async       Task                    ConnectAsync()
        {
            // Init
            lock (this) {
                if (_state != ConnectionState.Closed)
                    throw new SMPPException("SMPPConnection busy.");

                _tcpClient        = new TcpClient();
                _sendLock         = new TaskLock();
                _error            = null;
                _sequence         = 1;
            }

            Exception   error = null;

            // Connect to SMPP server
            try {
                Stream      stream;

                using (new System.Threading.Timer((object state) =>
                                                    {
                                                        lock(this) {
                                                            if (_state == ConnectionState.Connecting || _state == ConnectionState.SslHandshake) {
                                                                error = new TimeoutException("Timeout");
                                                                _tcpClient.Close();
                                                            }
                                                        }
                                                    },
                                                    null, 15 * 1000, System.Threading.Timeout.Infinite))
                {
                    _setState(ConnectionState.Connecting);
                    await _tcpClient.ConnectAsync(Hostname, Port);

                    stream = _tcpClient.GetStream();

                    if (Tls) {
                        _setState(ConnectionState.SslHandshake);
                        var sslStream = new SslStream(stream, true);
                        await sslStream.AuthenticateAsClientAsync(Hostname);
                        stream = sslStream;
                    }
                }

                lock(this) {
                    if (error != null)
                        throw error;

                    _setState(ConnectionState.StreamConnected);
                    _stream = stream;
                }
            }
            catch(Exception err) {
                if (err is ObjectDisposedException || err is NullReferenceException) {
                    lock(this) {
                        if (error != null)
                            err = error;
                        else
                        if (_state == ConnectionState.Closed)
                            err = new SMPPException("Connect aborted by close.");
                    }
                }

                err = _setFailed(new SMPPException("Connect failed.", err));
                Close();
                throw err;
            }

            // Start comtask
            var _ = _run();

            // Bind
            try {
                _setState(ConnectionState.Binding);
                var response = await _submitMessage(Bind);

                if (response.Status != CommandStatus.ESME_ROK)
                    throw new SMPPException("Response from server " + response.Status + ".");

                _setState(ConnectionState.Connected);
                _enquirePoll = EnquirePoll;
            }
            catch(Exception err) {
                throw _setFailed(new SMPPException("Bind failed", err));
            }
        }
        public              Task<SMPPMessage>       SubmitMessageAsync(SMPPMessage message)
        {
            if (_state != ConnectionState.Connected)
                throw new SMPPException("Not connected.");

            return _submitMessage(message);
        }
        public  async       Task                    StopAsync()
        {
            ConnectionState curState;

            lock(this) {
                if ((curState = _state) != ConnectionState.Connected) {
                    _setState(ConnectionState.Unbinding);
                }
            }

            try {
                if (curState == ConnectionState.Connected) {
                    var response = await _submitMessage(new SMPPUnbind());

                    if (response.Status != CommandStatus.ESME_ROK) {
                        throw new SMPPException("Response from server " + response.Status + ".");
                    }
                }
            }
            catch(Exception err) {
                throw _setFailed(new SMPPException("Unbind failed.", err));
            }

            // Wait until read has finished.
            for (var i = 0 ; i < 100 ; ++i) {
                if (_state == ConnectionState.Closed) {
                    return;
                }
                await Task.Delay(25);
            }

            // Force close
            Close();
        }
        public              void                    Close()
        {
            _closeTcpClient();
            _activeRequests.ConnectionDown();
            _sendLock.Dispose();
            _setState(ConnectionState.Closed);
        }

        private async       Task                    _run()
        {
            try {
                _enquirePoll = int.MaxValue;

                using (new System.Threading.Timer(_poll, null, 1000, 1000)) {
                    while (_isRunning) {
                        var message = await _recvMessage();

                        switch (message.Command) {
                        case CommandSet.EnquireLink:
                            _recvEnquireLink((SMPPEnquireLink)message);
                            break;

                        case CommandSet.DeliverSm:
                            _recvDeliverSm((SMPPDeliverSm)message);
                            break;

                        case CommandSet.Unbind:
                            await _recvUnbind((SMPPUnbind)message);
                            break;

                        case CommandSet.GenericNack:
                        case CommandSet.BindReceiverResp:
                        case CommandSet.BindTransceiverResp:
                        case CommandSet.BindTransmitterResp:
                        case CommandSet.SubmitSmResp:
                        case CommandSet.EnquireLinkResp:
                            _activeRequests.CompleteResp(message);
                            break;

                        case CommandSet.UnbindResp:
                            _activeRequests.CompleteResp(message);
                            _setState(ConnectionState.Stopped);
                            break;

                        default: throw new NotImplementedException("No handler for " + message.Command);
                        }
                    }
                }
            }
            catch(Exception err) {
                if (_isRunning) {
                    _setFailed(err);
                }
            }
            Close();
        }
        private             void                    _poll(object state)
        {
            _activeRequests.TimeoutPoll();

            if (_state == ConnectionState.Connected) {
                if (--_enquirePoll < 0) {
                    _enquirePoll = int.MaxValue;
                    _cmdEnquire();
                }
            }
        }
        private async       void                    _cmdEnquire()
        {
            try {
                if (_state == ConnectionState.Connected) {
                    var response = await _submitMessage(new SMPPEnquireLink());

                    if (response.Status != CommandStatus.ESME_ROK)
                        throw new SMPPException("Response from server " + response.Status + ".");

                    _enquirePoll = EnquirePoll;
                }
            }
            catch(Exception err) {
                lock(this) {
                    if (_state == ConnectionState.Connected) {
                        _setFailed(new SMPPException("EnquireLink failed", err));
                        _tcpClient?.Client.Close();
                    }
                }
            }
        }
        private async       void                    _recvDeliverSm(SMPPDeliverSm message)
        {
            if (OnDeliverSm != null) {
                await OnDeliverSm(message);
            }

            _sendMessageAsync(new SMPPDeliverSmResp(message.Sequence));
        }
        private             void                    _recvEnquireLink(SMPPEnquireLink message)
        {
            _sendMessageAsync(new SMPPEnquireLinkResp(message.Sequence));
        }
        private async       Task                    _recvUnbind(SMPPUnbind message)
        {
            await _sendMessage(new SMPPUnbindResp(message.Sequence));
            await Task.Delay(1000);
            throw new SMPPException("UNBind received from remote.");
        }
        private             Task<SMPPMessage>       _submitMessage(SMPPMessage message)
        {
            var sendingMessage = _activeRequests.AddMessage(message);

            sendingMessage.SendTask = _sendMessage(message);
            sendingMessage.SendTask.ContinueWith((task) =>
                                                 {
                                                    if (task.Status != TaskStatus.RanToCompletion)
                                                        _activeRequests.CompleteError(sendingMessage, task.Exception);
                                                  });
            return sendingMessage.TaskCompletion.Task;
        }
        private async       Task<SMPPMessage>       _recvMessage()
        {
            var buf = new byte[16];

            await _streamRead(buf, 0, buf.Length);

            var commandLength = PduReader.ParseInteger(buf, 0);

            if (commandLength < 16 || commandLength > (1 << 18))
                throw new SMPPException("Invalid command_length " + commandLength + "received.");

            if (commandLength > 16) {
                Array.Resize(ref buf, (int)commandLength);
                await _streamRead(buf, 16, (int)commandLength - 16);
            }

            var pduReader = new PduReader(buf);
#if DEBUG
            Debug.WriteLine("SMPPConnection: RecvMessage size=" + pduReader.CommandLength + " id=" + pduReader.CommandId + " status=" + pduReader.CommandStatus + " seq=" + pduReader.CommandSequence);
#endif
            try {
                switch (pduReader.CommandId) {
                case CommandSet.GenericNack:            return new SMPPGenericNack(pduReader);
                case CommandSet.BindReceiverResp:       return new SMPPBindResp(CommandSet.BindReceiverResp,    pduReader);
                case CommandSet.BindTransceiverResp:    return new SMPPBindResp(CommandSet.BindTransceiverResp, pduReader);
                case CommandSet.BindTransmitterResp:    return new SMPPBindResp(CommandSet.BindTransmitterResp, pduReader);
                case CommandSet.SubmitSmResp:           return new SMPPSubmitSmResp(pduReader);
                case CommandSet.DeliverSm:              return new SMPPDeliverSm(pduReader);
                case CommandSet.EnquireLink:            return new SMPPEnquireLink(pduReader);
                case CommandSet.EnquireLinkResp:        return new SMPPEnquireLinkResp(pduReader);
                case CommandSet.Unbind:                 return new SMPPUnbind(pduReader);
                case CommandSet.UnbindResp:             return new SMPPUnbindResp(pduReader);

                default:
                    throw new SMPPException("Invalid command.");
                    // TODO set nack
                }
            }
            catch(Exception err) {
                throw new SMPPPDUException("Error while parsing received PDU command " + pduReader.CommandId.ToString() + ".", buf, err);
            }
        }
        private async       Task                    _sendMessage(SMPPMessage message)
        {
            using (await _sendLock.Enter()) {
                if (!_isRunning)
                    throw new SMPPException("Connection is down.");

                var writer = new PduWriter();

                if ((message.Command & CommandSet.Response) == 0) {
                    lock(this) {
                        message.Sequence = _sequence;
                        _sequence = (_sequence < 1000000000) ? _sequence + 1 : 1;
                    }
                }

                writer.WriteMessage(message);

                var pduData = writer.PduData();
#if DEBUG
                Debug.WriteLine("SMPPConnection: SendMessage size=" + pduData.Length + " id=" + message.Command + " status=" + message.Status + " seq=" + message.Sequence);
#endif
                try {
                    await _stream.WriteAsync(new ReadOnlyMemory<byte>(pduData, 0, pduData.Length));
                }
                catch(Exception err) {
                    throw _setFailed(new SMPPException("Send data to SMPP server failed", err));
                }
            }
        }
        private async       void                    _sendMessageAsync(SMPPMessage message)
        {
            try {
                await _sendMessage(message);
            }
            catch(Exception err) {
                if (_isRunning) {
                    _setFailed(new SMPPException("SendMessage failed.", err));
                }
            }
        }
        private async       Task                    _streamRead(byte[] buf, int offset, int size)
        {
            int     rs;

            while (size > 0 && (rs = await _stream.ReadAsync(new Memory<byte>(buf, offset, size))) > 0) {
                offset += rs;
                size   -= rs;
            }

            if (size > 0)
                throw new SMPPException("Connection closed by remote.");
        }
        private             void                    _closeTcpClient()
        {
            lock(this) {
                if (_tcpClient != null) {
                    try {
                        _tcpClient.Close();
                        _stream?.Dispose();
                    }
                    catch(Exception err) {
                        Debug.WriteLine("SMPPConnection: close failed: " + err.Message);
                    }

                    _tcpClient = null;
                    _stream    = null;
                }
            }
        }
        private             void                    _setState(ConnectionState newState)
        {
            var prevState = ConnectionState.Unknown;

            lock(this) {
                if (_tcpClient == null && newState != ConnectionState.Closed) {
                    return ;
                }

                if (_state != newState) {
                    prevState = _state;
                    _state = newState;
                }
            }

            if (prevState != ConnectionState.Unknown) {
#if DEBUG
                Debug.WriteLine("SMPPConnection: state=" + newState);
#endif
                if (OnStateChange != null) {
                    try {
                        OnStateChange(prevState, newState);
                    }
                    catch(Exception) {
                    }
                }
            }
        }
        private             Exception               _setFailed(Exception err)
        {
            var prevState = ConnectionState.Unknown;

            lock(this) {
                if (_state != ConnectionState.Failed) {
                    prevState = _state;
                    _error    = err;
                    _state    = ConnectionState.Failed;
                }
            }

            if (prevState != ConnectionState.Unknown) {
#if DEBUG
                Debug.WriteLine("SMPPConnection: state=" + ConnectionState.Failed);
#endif
                if (OnStateChange != null) {
                    try {
                        OnStateChange(prevState, ConnectionState.Failed);
                    }
                    catch(Exception) {
                    }
                }
            }

            _closeTcpClient();

            return err;
        }
    }
}
