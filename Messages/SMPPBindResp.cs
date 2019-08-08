using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPBindResp: SMPPMessageWithOptional
    {
        private readonly        CommandSet          _command;

        public  override        CommandSet          Command                     { get { return _command; } }
        public                  string              SystemId                    { get; set; }


        internal                                    SMPPBindResp(CommandSet command, PduReader reader): base(reader)
        {
            _command = command;
            SystemId = reader.ReadCStringAscii();
            ReadOptional(reader);
        }
    }
}
