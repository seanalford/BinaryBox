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
    }
}
