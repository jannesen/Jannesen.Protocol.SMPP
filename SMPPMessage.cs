using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public abstract class SMPPMessage
    {
        public  abstract        CommandSet      Command         { get; }
        public                  CommandStatus   Status          { get; set; }
        public                  UInt32          Sequence        { get; set; }

        protected                               SMPPMessage()
        {
        }
        protected                               SMPPMessage(UInt32 sequence)
        {
            Sequence = sequence;
        }
        internal                                SMPPMessage(PduReader reader)
        {
            Status   = reader.CommandStatus;
            Sequence = reader.CommandSequence;
        }
        internal    virtual     void            Serialize(PduWriter writer)
        {
            throw new SMPPException("Can't send message " + Command + ".");
        }
    }
    public abstract class SMPPMessageWithOptional: SMPPMessage
    {
        private                 SMPPTLVCollection   _optional;

        public                  SMPPTLVCollection   Optional
        {
            get {
                if (_optional == null)
                    _optional = new SMPPTLVCollection();

                return _optional;
            }
        }
        public                  bool            hasOptional
        {
            get {
                return _optional != null;
            }
        }

        public                                  SMPPMessageWithOptional()
        {
        }
        internal                                SMPPMessageWithOptional(PduReader reader): base(reader)
        {
        }

        internal                void            ReadOptional(PduReader reader)
        {
            while (reader.SizeLeft > 0)
                Optional.Add(new SMPPTLV(reader));
        }
    }
}
