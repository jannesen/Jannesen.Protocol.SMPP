using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPSubmitSmResp: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return CommandSet.SubmitSmResp; } }
        public                  string              MessageId                   { get; set; }

        internal                                    SMPPSubmitSmResp(PduReader reader): base(reader)
        {
            MessageId = reader.ReadCStringAscii();
        }
    }
}
