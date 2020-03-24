namespace BinaryBox.Checksum
{
    public static class CrcExtentions
    {
        public static byte[] Crc16(this byte[] data)
        {
            byte[] result = new byte[2];
            ushort crc = 0xFFFF;

            for (int i = 0; i < data.Length - 2; i++)
            {
                crc ^= data[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 0x01) == 1)
                        crc = (ushort)((crc >> 1) ^ 0xA001);
                    else
                        crc = (ushort)(crc >> 1);
                }
            }

            result[0] = (byte)(crc & 0xFF);
            result[1] = (byte)((crc >> 8) & 0xFF);

            return result;
        }
    }
}
