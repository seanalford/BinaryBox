using System;
using System.Text;

namespace Toolbox.Checksum
{
    public static class LrcExtensions
    {
        public static byte ToByte(this byte[] toEncode)
        {
            byte LRC = 0;
            for (int i = 0; i < toEncode.Length; i++)
            {
                LRC ^= toEncode[i];
            }
            return LRC;
        }
        public static byte ToByte(this string toEncode)
        {
            return ToByte(Encoding.ASCII.GetBytes(toEncode));
        }

        public static char ToChar(this byte[] toEncode)
        {
            return Convert.ToChar(ToByte(toEncode));
        }

        public static char ToChar(this string toEncode)
        {
            return Convert.ToChar((ToByte(Encoding.ASCII.GetBytes(toEncode))));
        }

    }
}
