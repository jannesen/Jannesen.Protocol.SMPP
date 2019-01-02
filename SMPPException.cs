using System;
using System.Runtime.Serialization;

namespace Jannesen.Protocol.SMPP
{
    [Serializable]
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

    [Serializable]
    public class SMPPPDUException: SMPPException
    {
        public              byte[]          PDU         { get; private  set; }

        public                              SMPPPDUException(string message, byte[] pdu, Exception innerException): base(message, innerException)
        {
            PDU = pdu;
        }

        public  override    void            GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("PDU", PDU);
        }

        public  override    string          Source
        {
            get {
                return "Jannesen.Protocol.SMPP";
            }
        }
    }
}
