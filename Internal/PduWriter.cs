using System;

namespace Jannesen.Protocol.SMPP.Internal
{
    internal class PduWriter
    {
        private             byte[]      _data;
        private             int         _offset;

        public                          PduWriter()
        {
            _data   = new byte[4096];
            _offset = 0;
        }

        public              void        WriteMessage(SMPPMessage message)
        {
            WriteHeader         (message);

            message.Serialize(this);

            if (message is SMPPMessageWithOptional && ((SMPPMessageWithOptional)message).hasOptional)
                WriteOptional       (((SMPPMessageWithOptional)message).Optional);
        }

        public              void        WriteByte(byte value)
        {
            _requireSize(1);
            _data[_offset++] = value;
        }
        public              void        WriteInteger16(UInt16 value)
        {
            _requireSize(2);
            _data[_offset + 0] = (byte)(value >> 8);
            _data[_offset + 1] = (byte)(value     );
            _offset += 2;
        }
        public              void        WriteInteger32(UInt32 value)
        {
            _requireSize(4);
            _data[_offset + 0] = (byte)(value >> 24);
            _data[_offset + 1] = (byte)(value >> 16);
            _data[_offset + 2] = (byte)(value >>  8);
            _data[_offset + 3] = (byte)(value      );
            _offset += 4;
        }
        public              void        WriteBytes(byte[] value)
        {
            _requireSize(value.Length);
            Array.Copy(value, 0, _data, _offset, value.Length);
            _offset += value.Length;
        }
        public              void        WriteCStringAscii(string value, int minLength, int maxLength)
        {
            if (value != null) {
                if (value.Length < minLength || value.Length > maxLength)
                    throw new ArgumentException("Invalid string length.");

                byte[]  b = System.Text.ASCIIEncoding.ASCII.GetBytes(value);

                _requireSize(b.Length + 1);
                Array.Copy(b, 0, _data, _offset, b.Length);
                _offset += b.Length + 1;
            }
            else {
                if (minLength > 0)
                    throw new ArgumentException("Invalid string length.");

                WriteByte(0);
            }
        }

        public              void        WriteHeader(SMPPMessage message)
        {
            _offset = 4;
            WriteInteger32((UInt32)message.Command);
            WriteInteger32((UInt32)message.Status);
            WriteInteger32(message.Sequence);
        }
        public              void        WriteOptional(SMPPTLVCollection tlvCollection)
        {
            foreach(SMPPTLV tlv in tlvCollection) {
                WriteInteger16((UInt16)tlv.Tag);

                if (tlv.Value != null) {
                    WriteInteger16((UInt16)tlv.Value.Length);
                    WriteBytes(tlv.Value);
                }
                else
                    WriteInteger16(0);
            }
        }

        public              byte[]      PduData()
        {
            _data[0] = (byte)(_offset >> 24);
            _data[1] = (byte)(_offset >> 16);
            _data[2] = (byte)(_offset >>  8);
            _data[3] = (byte)(_offset      );

            Array.Resize(ref _data, _offset);

            return _data;
        }

        private             void        _requireSize(int size)
        {
            if (_offset + size > _data.Length) {
                Array.Resize(ref _data, _data.Length + 4096);
            }
        }
    }
}
