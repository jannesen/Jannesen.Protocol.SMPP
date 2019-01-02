using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPUnbind: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return CommandSet.Unbind; } }

        public                                      SMPPUnbind()
        {
        }
        internal                                    SMPPUnbind(PduReader reader): base(reader)
        {
        }

        internal    override    void                Serialize(PduWriter writer)
        {
        }
    }
}
