using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace Genshin.Protobuf
{
    public class Message
    {
        private Dictionary<int, Field> fields = new Dictionary<int, Field>();

        public bool Valid { get; private set; }

        public static Message Parse(byte[] binary)
        {
            Message message = new Message();
            message.ParseFromBytes(binary);
            return message;
        }

        public void ParseFromBytes(byte[] binary)
        {
            try
            {
                ProtoReader pr = new ProtoReader(binary);
                while (!pr.IsEos())
                {
                    (WireType wt, int fieldNumber) = pr.ReadWireTypeAndFieldNumber();
                    switch (wt)
                    {
                        case WireType.VarInt:
                        {
                            Field field = AddField(fieldNumber, FieldType.UInt64);
                            field.AddRaw(pr.ReadVarInt());
                            break;
                        }
                        case WireType.Bit64:
                        {
                            Field field = AddField(fieldNumber, FieldType.UInt64);
                            field.AddRaw(pr.ReadUInt64());
                            break;
                        }
                        case WireType.LengthLimited:
                        {
                            Field field = AddField(fieldNumber, FieldType.Bytes);
                            ulong size = pr.ReadVarInt();
                            field.AddRaw(pr.ReadBytes((int) size));
                            break;
                        }
                        case WireType.GroupStart:
                        case WireType.GroupEnd:
                        {
                            return;
                        }
                        case WireType.Bit32:
                        {
                            Field field = AddField(fieldNumber, FieldType.UInt32);
                            field.AddRaw(pr.ReadUInt32());
                            break;
                        }
                        default:
                        {
                            return;
                        }
                    }
                }

                Valid = true;
            }
            catch (Exception)
            {
                Valid = false;
            }
        } 

        public Field AddField(int fieldNumber, FieldType fieldType)
        {
            if (!fields.ContainsKey(fieldNumber))
                fields[fieldNumber] = new Field(fieldType);
            if (fields[fieldNumber].Type != fieldType)
                throw new InvalidDataException();
            return fields[fieldNumber];
        }

        public Field this[int fieldNumber] => fields[fieldNumber];

        public bool ContainsField(int fieldNumber)
        {
            return fields.ContainsKey(fieldNumber);
        } 

        public string ToString(int indent)
        { 
            StringBuilder sb = new StringBuilder();
            foreach (var f in fields)
            {
                sb.AppendLine(new string('\t', indent) + $"{f.Key}: {f.Value.Type}");
                sb.Append($"{f.Value.ToString(indent + 1)}");
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return ToString(0);
        }

        public void PrintToDebug()
        {
            Debug.WriteLine(this);
        }
    }
}
