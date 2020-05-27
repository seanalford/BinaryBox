using BinaryBox.Checksum;
using System.ComponentModel;

namespace BinaryBox.ProtocolV2.Settings
{
    public abstract class ProtocolSettings : IProtocolSettings
    {
        public const int DEFAULT_BUFFER_SIZE = 1024;
        public const ChecksumTypes DEFAULT_CHECKSUM_TYPE = ChecksumTypes.None;
        public const int DEFAULT_CONNECT_RETIRES = 3;
        public const int DEFAULT_OPEN_CLOSE_TIMEOUT = 15000;
        public const int DEFAULT_PRIMARY_READ_TIMEOUT = 15000;
        public const int DEFAULT_RECEIVE_RETRIES = 3;
        public const int DEFAULT_SEND_RETIRES = 3;
        public const int DEFAULT_SECONDARY_READ_TIMEOUT = 1000;
        public const int DEFAULT_WRITE_TIMEOUT = 15000;

        public event PropertyChangedEventHandler PropertyChanged;

        public int BufferSize { get; set; } = DEFAULT_BUFFER_SIZE;
        public ChecksumTypes Checksum { get; set; } = DEFAULT_CHECKSUM_TYPE;
        public int ConnectRetries { get; set; } = DEFAULT_CONNECT_RETIRES;
        public int OpenCloseTimeout { get; set; } = DEFAULT_OPEN_CLOSE_TIMEOUT;
        public int PrimaryReadTimeout { get; set; } = DEFAULT_PRIMARY_READ_TIMEOUT;
        public int ReceiveRetries { get; set; } = DEFAULT_RECEIVE_RETRIES;
        public int SendRetries { get; set; } = DEFAULT_SEND_RETIRES;
        public int SecondaryReadTimeout { get; set; } = DEFAULT_SECONDARY_READ_TIMEOUT;
        public int WriteTimeout { get; set; } = DEFAULT_WRITE_TIMEOUT;

        public void Dispose()
        {

        }
    }
}
