using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Genshin.Protobuf
{
    public class Field
    {
        public FieldType Type;

        private List<object> value = new List<object>();

        private Dictionary<int, Message> cachedMessages = new Dictionary<int, Message>();

        public Field(FieldType type)
        {
            Type = type;
        }

        public void AddRaw(object value)
        {
            this.value.Add(value);
        }

        private void CheckType(FieldType type)
        {
            if (type != Type)
                throw new InvalidDataException();
        }

        private T GetRaw<T>(int index)
        {
            return (T) value[index];
        }

        private int GetInt32Value(int index = 0)
        {
            CheckType(FieldType.UInt32);

            uint val = GetRaw<uint>(index);
            int ret = -(int) (val & 1) ^ (int) (val >> 1);
            return ret;
        }

        private uint GetUInt32Value(int index = 0)
        {
            CheckType(FieldType.UInt32);

            uint val = GetRaw<uint>(index);
            return val;
        }

        private long GetInt64Value(int index = 0)
        {
            CheckType(FieldType.UInt64);

            ulong val = GetRaw<ulong>(index);
            long ret = -(long) (val & 1) ^ (long) (val >> 1);
            return ret;
        }

        private ulong GetUInt64Value(int index = 0)
        {
            CheckType(FieldType.UInt64);

            ulong val = GetRaw<ulong>(index);
            return val;
        }

        private unsafe float GetFloatValue(int index = 0)
        {
            uint val = GetUInt32Value(index);
            uint* pval = &val;
            float ret = *((float*) pval);
            return ret;
        }

        private unsafe double GetDoubleValue(int index = 0)
        {
            ulong val = GetUInt64Value(index);
            ulong* pval = &val;
            double ret = *((double*) pval);
            return ret;
        }

        private byte[] GetBytesValue(int index = 0)
        {
            CheckType(FieldType.Bytes);

            return GetRaw<byte[]>(index);
        }

        private string GetStringValue(int index = 0)
        {
            return Encoding.UTF8.GetString(GetBytesValue(index));
        }

        public Message GetMessageValue(int index = 0)
        {
            if (!cachedMessages.ContainsKey(index))
            {
                byte[] val = GetBytesValue(index);
                Message subMessage = new Message();
                subMessage.ParseFromBytes(val);
                cachedMessages[index] = subMessage;
            }

            return cachedMessages[index];
        }

        public int Int32Value => GetInt32Value();
        public uint UInt32Value => GetUInt32Value();
        public long Int64Value => GetInt64Value();
        public ulong UInt64Value => GetUInt64Value();
        public float FloatValue => GetFloatValue();
        public double DoubleValue => GetDoubleValue();
        public byte[] BytesValue => GetBytesValue();
        public string StringValue => GetStringValue();
        public Message MessageValue => GetMessageValue();

        public IEnumerable<int> Int32Array => value.Select((val, idx) => GetInt32Value(idx));
        public IEnumerable<uint> UInt32Array => value.Select((val, idx) => GetUInt32Value(idx));
        public IEnumerable<long> Int64Array => value.Select((val, idx) => GetInt64Value(idx));
        public IEnumerable<ulong> UInt64Array => value.Select((val, idx) => GetUInt64Value(idx));
        public IEnumerable<float> FloatArray => value.Select((val, idx) => GetFloatValue(idx));
        public IEnumerable<double> DoubleArray => value.Select((val, idx) => GetDoubleValue(idx));
        public IEnumerable<byte[]> BytesArray => value.Select((val, idx) => GetBytesValue(idx));
        public IEnumerable<string> StringArray => value.Select((val, idx) => GetStringValue(idx));
        public IEnumerable<Message> MessageArray => value.Select((val, idx) => GetMessageValue(idx));

        public string ToString(int indent)
        {
            StringBuilder sb = new StringBuilder();
            switch (Type)
            {
                case FieldType.UInt32:
                {
                    for (int i = 0; i < value.Count; ++i)
                    {
                        sb.AppendLine(new string('\t', indent) +
                                      $"{GetInt32Value(i)} / {GetUInt32Value(i)} / {GetUInt32Value(i):X} / {GetFloatValue(i)}");
                    }

                    break;
                }
                case FieldType.UInt64:
                {
                    for (int i = 0; i < value.Count; ++i)
                    {
                        sb.AppendLine(new string('\t', indent) +
                                      $"{GetInt64Value(i)} / {GetUInt64Value(i)} / {GetUInt64Value(i):X} / {GetDoubleValue(i)}");
                    }

                    break;
                }
                case FieldType.Bytes:
                {
                    for (int i = 0; i < value.Count; ++i)
                    {
                        byte[] val = GetBytesValue(i);
                        Message tryMessage = Message.Parse(val);
                        if (tryMessage.Valid)
                        {
                            sb.Append(tryMessage.ToString(indent));
                        }
                        else
                        {
                            sb.AppendLine(new string('\t', indent) + $"{GetStringValue(i)}"); 
                            sb.AppendBytes(val, indent);
                        }
                    }

                    break;
                }
            }

            return sb.ToString();
        }
    }
}