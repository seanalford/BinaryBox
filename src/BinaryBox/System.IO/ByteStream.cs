using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Core.System.IO
{
    public abstract class ByteStream : IByteStream
    {
        public ILogger Log { get; private set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public IByteStreamSettings Settings { get; set; }
        public ByteStreamState State { get; protected set; }

        public ByteStream(ILogger logger, IByteStreamSettings settings)
        {
            Log = logger;
            Settings = settings;
        }

        public abstract Task<ByteStreamResponse<ByteStreamState>> CloseAsync(CancellationToken cancellationToken = default);
        public abstract Task<ByteStreamResponse<bool>> DataAvailableAsync();
        public abstract void Dispose();
        public abstract Task<ByteStreamResponse<ByteStreamState>> OpenAsync(CancellationToken cancellationToken = default);
        public abstract Task<ByteStreamResponse<int>> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
        public abstract Task<ByteStreamResponse<bool>> WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
    }
}
