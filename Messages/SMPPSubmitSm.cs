using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPSubmitSm: SMPPMessageWithOptional
    {
        public  override        CommandSet              Command                     { get { return CommandSet.SubmitSm; } }
        public                  string                  ServiceType                 { get; set; }
        public                  TonType                 SourceTon                   { get; set; }
        public                  NpiType                 SourceNpi                   { get; set; }
        public                  string                  SourceAddr                  { get; set; }
        public                  TonType                 DestTon                     { get; set; }
        public                  NpiType                 DestNpi                     { get; set; }
        public                  string                  DestAddr                    { get; set; }
        public                  EmsClass                EsmClass                    { get; set; }
        public                  byte                    ProtocolId                  { get; set; }
        public                  PriorityFlags           PriorityFlag                { get; set; }
        public                  string                  ScheduleDeliveryTime        { get; set; }
        public                  string                  ValidityPeriod              { get; set; }
        public                  RegisteredDeliveryFlags RegisteredDelivery          { get; set; }
        public                  byte                    ReplaceIfPresent            { get; set; }
        public                  DataCodings             DataCoding                  { get; set; }
        public                  byte                    DefaultMessageId            { get; set; }
        public                  byte[]                  ShortMessage                { get; set; }

        internal    override    void                Serialize(PduWriter writer)
        {
            writer.WriteCStringAscii    (ServiceType,           0, 5);
            writer.WriteByte            ((byte)SourceTon);
            writer.WriteByte            ((byte)SourceNpi);
            writer.WriteCStringAscii    (SourceAddr,            0, 20);
            writer.WriteByte            ((byte)DestTon);
            writer.WriteByte            ((byte)DestNpi);
            writer.WriteCStringAscii    (DestAddr,              0, 20);
            writer.WriteByte            ((byte)EsmClass);
            writer.WriteByte            ((byte)ProtocolId);
            writer.WriteByte            ((byte)PriorityFlag);
            writer.WriteCStringAscii    (ScheduleDeliveryTime,  0, 16);
            writer.WriteCStringAscii    (ValidityPeriod,        0, 16);
            writer.WriteByte            ((byte)RegisteredDelivery);
            writer.WriteByte            (ReplaceIfPresent);
            writer.WriteByte            ((byte)DataCoding);
            writer.WriteByte            (DefaultMessageId);
            writer.WriteByte            ((byte)ShortMessage.Length);
            writer.WriteBytes           (ShortMessage);
        }
    }
}
