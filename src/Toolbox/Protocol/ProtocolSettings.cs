using Toolbox.Checksum;

namespace Toolbox.Protocol
{
    public class ProtocolSettings : IProtocolSettings
    {
        private const int DEFAULT_CONNECT_RETIRES = 3;
        private const int DEFAULT_RECEIVE_RETRIES = 3;
        private const int DEFAULT_SEND_RETIRES = 3;

        public ChecksumTypes Checksum { get; set; } = ChecksumTypes.None;
        public int ConnectRetries { get; set; } = DEFAULT_CONNECT_RETIRES;
        public int ReceiveRetries { get; set; } = DEFAULT_RECEIVE_RETRIES;
        public int SendRetries { get; set; } = DEFAULT_SEND_RETIRES;
    }
}
