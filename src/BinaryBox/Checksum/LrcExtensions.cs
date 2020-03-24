namespace BinaryBox.Checksum
{
    public static class LrcExtensions
    {
        public static byte Lrc(this byte[] data)
        {
            byte LRC = 0;
            for (int i = 0; i < data.Length; i++)
            {
                LRC ^= data[i];
            }
            return LRC;
        }
    }
}
