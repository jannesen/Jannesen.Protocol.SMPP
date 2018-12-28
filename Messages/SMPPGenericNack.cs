using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPGenericNack: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return CommandSet.GenericNack; } }

        internal                                    SMPPGenericNack(PduReader reader): base(reader)
        {
        }
    }
}
