using System;

namespace Jannesen.Protocol.SMPP.Internal
{
    internal sealed class PduReader
    {
        private readonly    byte[]          _data;
        private readonly    int             _length;
        private             int             _offset;

        public              UInt32          CommandLength
        {
            get {
                return ParseInteger(_data, 0);
            }
        }
        public              CommandSet      CommandId
        {
            get {
                return (CommandSet)ParseInteger(_data, 4);
            }
        }
        public              CommandStatus   CommandStatus
        {
            get {
                return (CommandStatus)ParseInteger(_data, 8);
            }
        }
        public              UInt32          CommandSequence
        {
            get {
                return ParseInteger(_data, 12);
            }
        }
        public              int             SizeLeft
        {
            get {
                return _length - _offset;
            }
        }

        public                              PduReader(byte[] data)
        {
            _data   = data;
            _length = data.Length;
            _offset = 16;
        }

        public              byte            ReadByte()
        {
            return _data[_offset++];
        }
        public              UInt16          ReadInteger16()
        {
            UInt16  v =  (UInt16)((_data[_offset + 0] <<  8) |
                                  (_data[_offset + 1]      ));

            _offset += 2;

            return v;
        }
        public              UInt32          ReadInteger32()
        {
            UInt32  v = ParseInteger(_data, _offset);

            _offset += 4;

            return v;
        }
        public              byte[]          ReadBytes(int size)
        {
            byte[]  rtn = new byte[size];

            Array.Copy(_data, _offset, rtn, 0, size);
            _offset += size;

            return rtn;
        }
        public              string          ReadCStringAscii()
        {
            int pos = _offset;

            while (pos < _data.Length && _data[pos] != 0)
                ++pos;

            string s = System.Text.ASCIIEncoding.ASCII.GetString(_data, _offset, pos - _offset);

            _offset = pos + 1;

            return s;
        }
        public  static      UInt32          ParseInteger(byte[] buf, int offset)
        {
            return (UInt32)((buf[offset + 0] << 24) |
                            (buf[offset + 1] << 16) |
                            (buf[offset + 2] <<  8) |
                            (buf[offset + 3]      ));
        }
    }
}
