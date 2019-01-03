using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPDeliverSmResp: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return CommandSet.DeliverSmResp; } }
        public                  string              MessageId                   { get; set; }

        public                                      SMPPDeliverSmResp(UInt32 sequence): base(sequence)
        {
        }
        internal    override    void                Serialize(PduWriter writer)
        {
            writer.WriteCStringAscii    (MessageId,  0, 15);
        }
    }
}
