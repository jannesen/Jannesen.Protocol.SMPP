using System;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPException: Exception
    {
        public                              SMPPException(string message): base(message)
        {
        }
        public                              SMPPException(string message, Exception innerException): base(message, innerException)
        {
        }

        public  override    string          Source
        {
            get {
                return "Jannesen.Protocol.SMPP";
            }
        }
    }

    public class SMPPPDUException: SMPPException
    {
        public              byte[]          PDU         { get; private  set; }

        public                              SMPPPDUException(string message, byte[] pdu, Exception innerException): base(message, innerException)
        {
            PDU = pdu;
        }

        public  override    string          Source
        {
            get {
                return "Jannesen.Protocol.SMPP";
            }
        }
    }
}
