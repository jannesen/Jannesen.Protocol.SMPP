using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPEnquireLink: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return CommandSet.EnquireLink; } }

        public                                      SMPPEnquireLink()
        {
        }
        internal                                    SMPPEnquireLink(PduReader reader): base(reader)
        {
        }

        internal    override    void                Serialize(PduWriter writer)
        {
        }
    }
}
