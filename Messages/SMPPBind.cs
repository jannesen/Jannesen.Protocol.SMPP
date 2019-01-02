using System;
using Jannesen.Protocol.SMPP.Internal;

namespace Jannesen.Protocol.SMPP
{
    public class SMPPBind: SMPPMessage
    {
        public  override        CommandSet          Command                     { get { return (CommandSet)BindingType; } }
        public                  BindingType         BindingType                 { get; set; }
        public                  string              SystemId                    { get; set; }
        public                  string              Password                    { get; set; }
        public                  string              SystemType                  { get; set; }
        public                  SmppVersionType     InterfaceVersion            { get; set; }
        public                  TonType             AddrTon                     { get; set; }
        public                  NpiType             AddrNpi                     { get; set; }
        public                  string              AddressRange                { get; set; }

        public                                      SMPPBind()
        {
        }

        internal    override    void                Serialize(PduWriter writer)
        {
            writer.WriteCStringAscii    (SystemId,   1, 15);
            writer.WriteCStringAscii    (Password,   0,  8);
            writer.WriteCStringAscii    (SystemType, 0, 12);
            writer.WriteByte            ((byte)InterfaceVersion);
            writer.WriteByte            ((byte)AddrTon);
            writer.WriteByte            ((byte)AddrNpi);
            writer.WriteCStringAscii    (AddressRange, 0, 40);
        }
    }
}
