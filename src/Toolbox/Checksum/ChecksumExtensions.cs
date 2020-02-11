namespace Toolbox.Checksum
{
    public static partial class ChecksumExtensions
    {
        public static int Length(this ChecksumTypes checksum)
        {
            int result = 0;
            switch (checksum)
            {
                case ChecksumTypes.LRC:
                    result = 1;
                    break;
                case ChecksumTypes.CRC16:
                    result = 2;
                    break;
            }
            return result;
        }

        public static byte? ChecksumToByte(this byte[] toEcode, ChecksumTypes type)
        {
            byte? result = null;
            switch (type)
            {
                case ChecksumTypes.None:
                    break;
                case ChecksumTypes.LRC:
                    result = toEcode.ToByte();
                    break;
                case ChecksumTypes.CRC16:
                    break;
            }
            return result;
        }

        public static byte? ChecksumToByte(this string toEncode, ChecksumTypes type)
        {
            byte? result = null;
            switch (type)
            {
                case ChecksumTypes.None:
                    break;
                case ChecksumTypes.LRC:
                    result = toEncode.ToByte();
                    break;
                case ChecksumTypes.CRC16:
                    break;
            }
            return result;
        }

        public static char? ChecksumToChar(this byte[] toEncode, ChecksumTypes type)
        {
            char? result = null;
            switch (type)
            {
                case ChecksumTypes.None:
                    break;
                case ChecksumTypes.LRC:
                    result = toEncode.ToChar();
                    break;
                case ChecksumTypes.CRC16:
                    break;
            }
            return result;
        }

        public static char? ChecksumToChar(this string toEncode, ChecksumTypes type)
        {
            char? result = null;
            switch (type)
            {
                case ChecksumTypes.None:
                    break;
                case ChecksumTypes.LRC:
                    result = toEncode.ToChar();
                    break;
                case ChecksumTypes.CRC16:
                    break;
            }
            return result;
        }
    }
}
