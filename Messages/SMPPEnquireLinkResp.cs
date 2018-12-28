using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPEnquireLinkResp: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return CommandSet.EnquireLinkResp; } }

        public                                      SMPPEnquireLinkResp(UInt32 sequence): base(sequence)
        {
        }
        internal                                    SMPPEnquireLinkResp(PduReader reader): base(reader)
        {
        }
    }
}
