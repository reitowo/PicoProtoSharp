using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genshin.Protobuf
{
    public static class StringBuilderHelper
    {
        public static void AppendBytes(this StringBuilder sb, byte[] bytes, int indent = 0)
        {
            int idx = 0;
            string printable = string.Empty;
            foreach (var b in bytes)
            {
                if (idx % 16 == 0)
                {
                    if (idx != 0)
                    {
                        sb.AppendLine("\t\t" + printable);
                        printable = string.Empty;
                    }
                    sb.Append(new string('\t', indent)); 
                }

                idx++;
                sb.Append(b.ToString("X").PadLeft(2, '0') + " ");
                printable += (b >= 32 && b <= 127) ? new string((char)b, 1) : ".";
            }

            if (printable != string.Empty)
            {
                sb.AppendLine("\t\t" + printable); 
            }
        }
    }
}
