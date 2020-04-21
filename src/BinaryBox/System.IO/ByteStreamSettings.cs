using System.ComponentModel;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamSettings : IByteStreamSettings
    {
        public const int DEFAULT_OPEN_CLOSE_TIMEOUT = 15000;
        public const int DEFAULT_BUFFER_SIZE = 1024;
        public const int DEFAULT_PRIMARY_READ_TIMEOUT = 15000;
        public const int DEFAULT_SECONDARY_READ_TIMEOUT = 1000;
        public const int DEFAULT_WRITE_TIMEOUT = 15000;

        public event PropertyChangedEventHandler PropertyChanged;

        public int OpenCloseTimeout { get; set; } = DEFAULT_OPEN_CLOSE_TIMEOUT;
        public int BufferSize { get; set; } = DEFAULT_BUFFER_SIZE;
        public int ReadPrimaryTimeout { get; set; } = DEFAULT_PRIMARY_READ_TIMEOUT;
        public int ReadSecondaryTimeout { get; set; } = DEFAULT_SECONDARY_READ_TIMEOUT;
        public int WriteTimeout { get; set; } = DEFAULT_WRITE_TIMEOUT;

        public void Dispose()
        {

        }
    }

}
