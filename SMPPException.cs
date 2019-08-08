using System;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Permissions;

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
        protected                           SMPPException(SerializationInfo info, StreamingContext context): base(info, context)
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

        protected                           SMPPPDUException(SerializationInfo info, StreamingContext context): base(info, context)
        {
            PDU = (byte[])info.GetValue(nameof(PDU), typeof(byte[]));
        }
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public  override    void            GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(PDU), PDU);
        }

        public  override    string          Source
        {
            get {
                return "Jannesen.Protocol.SMPP";
            }
        }
    }
}
