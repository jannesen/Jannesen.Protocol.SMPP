using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPUnbindResp: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return CommandSet.UnbindResp; } }

        public                                      SMPPUnbindResp(UInt32 sequence): base(sequence)
        {
        }
        internal                                    SMPPUnbindResp(PduReader reader): base(reader)
        {
        }
    }
}
