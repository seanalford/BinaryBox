using System;
using System.Text;

namespace Toolbox.Checksum
{
    public static class LrcExtensions
    {
        public static byte LrcByte(this byte[] data)
        {
            byte LRC = 0;
            for (int i = 0; i < data.Length; i++)
            {
                LRC ^= data[i];
            }
            return LRC;
        }
        public static byte LrcByte(this string data)
        {
            return LrcByte(Encoding.ASCII.GetBytes(data));
        }

        public static char LrcChar(this byte[] data)
        {
            return Convert.ToChar(LrcByte(data));
        }

        public static char LrcChar(this string data)
        {
            return Convert.ToChar((LrcByte(Encoding.ASCII.GetBytes(data))));
        }

    }
}
