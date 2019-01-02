using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPDeliverSm: SMPPMessageWithOptional
    {
        public  override        CommandSet              Command                     { get { return CommandSet.DeliverSm; } }
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

        internal                                    SMPPDeliverSm(PduReader reader): base(reader)
        {
            ServiceType             =                           reader.ReadCStringAscii();
            SourceTon               = (TonType)                 reader.ReadByte();
            SourceNpi               = (NpiType)                 reader.ReadByte();
            SourceAddr              =                           reader.ReadCStringAscii();
            DestTon                 = (TonType)                 reader.ReadByte();
            DestNpi                 = (NpiType)                 reader.ReadByte();
            DestAddr                =                           reader.ReadCStringAscii();
            EsmClass                = (EmsClass)                reader.ReadByte();
            ProtocolId              =                           reader.ReadByte();
            PriorityFlag            = (PriorityFlags)           reader.ReadByte();
            ScheduleDeliveryTime    =                           reader.ReadCStringAscii();
            ValidityPeriod          =                           reader.ReadCStringAscii();
            RegisteredDelivery      = (RegisteredDeliveryFlags) reader.ReadByte();
            ReplaceIfPresent        =                           reader.ReadByte();
            DataCoding              = (DataCodings)             reader.ReadByte();
            DefaultMessageId        =                           reader.ReadByte();
            ShortMessage            =                           reader.ReadBytes(reader.ReadByte());
            ReadOptional(reader);
        }
    }
}
