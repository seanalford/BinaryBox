namespace Toolbox.Checksum
{
    public static partial class ChecksumExtensions
    {
        public static byte[] Checksum(this byte[] data, ChecksumTypes checksum)
        {
            // TODO: Add test.
            byte[] result = new byte[checksum.Length()];
            switch (checksum)
            {
                case ChecksumTypes.LRC:
                    result[0] = data.Lrc();
                    break;
                case ChecksumTypes.CRC16:
                    result = data.Crc16();
                    break;
            }
            return result;
        }

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
    }
}
