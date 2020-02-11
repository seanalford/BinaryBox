using ReactiveUI.Fody.Helpers;

namespace Toolbox.Connection
{
    public interface IConnectionSettings
    {
        Checksum.ChecksumTypes Checksum { get; set; }
        int ConnectRetries { get; set; }
        int ReceiveBufferSize { get; set; }
        int ReceiveRetries { get; set; }
        int ReceiveTimeoutInner { get; set; }
        int ReceiveTimeoutOuter { get; set; }
        int SendBufferSize { get; set; }
        int SendRetries { get; set; }
        int SendTimeout { get; set; }
    }

    public class ConnectionSettings : IConnectionSettings
    {
        public const int DEFAULT_CONNECT_RETIRES = 3000;
        public const int DEFAULT_RECEIVE_BUFFER_SIZE = 1024;
        public const int DEFAULT_RECEIVE_RETRIES = 3000;
        public const int DEFAULT_RECEIVE_TIMEOUT_INNER = 1000;
        public const int DEFAULT_RECEIVE_TIMEOUT_OUTER = 15000;
        public const int DEFAULT_SEND_BUFFER_SIZE = 1024;
        public const int DEFAULT_SEND_RETIRES = 3000;
        public const int DEFAULT_SEND_TIMEOUT = 15000;

        [Reactive] public Checksum.ChecksumTypes Checksum { get; set; } = Toolbox.Checksum.ChecksumTypes.None;
        [Reactive] public int ConnectRetries { get; set; } = DEFAULT_CONNECT_RETIRES;
        [Reactive] public int ReceiveBufferSize { get; set; } = DEFAULT_RECEIVE_BUFFER_SIZE;
        [Reactive] public int ReceiveRetries { get; set; } = DEFAULT_RECEIVE_RETRIES;
        [Reactive] public int ReceiveTimeoutInner { get; set; } = DEFAULT_RECEIVE_TIMEOUT_INNER;
        [Reactive] public int ReceiveTimeoutOuter { get; set; } = DEFAULT_RECEIVE_TIMEOUT_OUTER;
        [Reactive] public int SendBufferSize { get; set; } = DEFAULT_SEND_BUFFER_SIZE;
        [Reactive] public int SendRetries { get; set; } = DEFAULT_SEND_RETIRES;
        [Reactive] public int SendTimeout { get; set; } = DEFAULT_SEND_TIMEOUT;

    }
}
