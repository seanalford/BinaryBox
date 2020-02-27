using System.ComponentModel;

namespace Toolbox.Connection
{
    public class ConnectionSettings : IConnectionSettings, INotifyPropertyChanged
    {
        public const int DEFAULT_RECEIVE_BUFFER_SIZE = 1024;
        public const int DEFAULT_RECEIVE_TIMEOUT_INNER = 1000;
        public const int DEFAULT_RECEIVE_TIMEOUT_OUTER = 15000;
        public const int DEFAULT_SEND_BUFFER_SIZE = 1024;
        public const int DEFAULT_SEND_TIMEOUT = 15000;

        public event PropertyChangedEventHandler PropertyChanged;

        public int ReceiveBufferSize { get; set; } = DEFAULT_RECEIVE_BUFFER_SIZE;
        public int ReceiveTimeoutInner { get; set; } = DEFAULT_RECEIVE_TIMEOUT_INNER;
        public int ReceiveTimeoutOuter { get; set; } = DEFAULT_RECEIVE_TIMEOUT_OUTER;
        public int SendBufferSize { get; set; } = DEFAULT_SEND_BUFFER_SIZE;
        public int SendTimeout { get; set; } = DEFAULT_SEND_TIMEOUT;
    }
}
