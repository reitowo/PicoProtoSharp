using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Genshin.Protobuf
{
    public class ProtoReader : BinaryReader
    {   
        public bool IsEos()
        {  
            return BaseStream.Position >= BaseStream.Length;
        }

        public ProtoReader(byte[] binary) : base(new MemoryStream(binary))
        { 

        }

        public (WireType, int) ReadWireTypeAndFieldNumber()
        {
            byte val = ReadByte();
            WireType wireType = (WireType) (val & 0x07);
            int fieldNumber = val >> 3;
            if (fieldNumber >= 16)
            {
                fieldNumber = fieldNumber & 0xF;
                int shift = 4;
                while (true)
                {
                    byte next = ReadByte();
                    fieldNumber |= (next & 0x7F) << shift;
                    shift += 7;
                    if (next < 128)
                        break;
                }
            }

            return (wireType, fieldNumber);
        }

        public ulong ReadVarInt()
        {
            ulong ret = 0;
            int shift = 0;
            while (true)
            {
                byte next = ReadByte();
                ret += (ulong)(next & 0x7F) << shift;
                shift += 7;
                if (next < 128)
                    break;
            }

            return ret;
        } 
    }
}
