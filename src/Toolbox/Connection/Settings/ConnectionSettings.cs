using System.ComponentModel;

namespace Toolbox.Connection
{
    public class ConnectionSettings : IConnectionSettings, INotifyPropertyChanged
    {
        public const int DEFAULT_READ_BUFFER_SIZE = 1024;
        public const int DEFAULT_PRIMARY_READ_TIMEOUT = 1000;
        public const int DEFAULT_SECONDARY_READ_TIMEOUT = 15000;
        public const int DEFAULT_WRITE_BUFFER_SIZE = 1024;
        public const int DEFAULT_WRITE_TIMEOUT = 15000;

        public event PropertyChangedEventHandler PropertyChanged;

        public int ReadBufferSize { get; set; } = DEFAULT_READ_BUFFER_SIZE;
        public int PrimaryReadTimeout { get; set; } = DEFAULT_PRIMARY_READ_TIMEOUT;
        public int SecondaryReadTimeout { get; set; } = DEFAULT_SECONDARY_READ_TIMEOUT;
        public int WriteBufferSize { get; set; } = DEFAULT_WRITE_BUFFER_SIZE;
        public int WriteTimeout { get; set; } = DEFAULT_WRITE_TIMEOUT;
    }
}
