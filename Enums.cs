using System;
using System.Collections.Generic;
using System.Text;

namespace Genshin.Protobuf
{
    public enum WireType
    {
        VarInt,
        Bit64,
        LengthLimited,
        GroupStart,
        GroupEnd,
        Bit32
    }

    public enum FieldType
    {
        Unset,
        UInt32,
        UInt64,
        Bytes
    }
}
