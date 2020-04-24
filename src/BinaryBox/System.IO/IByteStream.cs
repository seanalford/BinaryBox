using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Core.System.IO
{
    public interface IByteStream : IDisposable
    {
        ILogger Log { get; }
        IByteStreamSettings Settings { get; set; }
        ByteStreamState State { get; }
        Task<ByteStreamResponse<ByteStreamState>> OpenAsync();
        Task<ByteStreamResponse<ByteStreamState>> CloseAsync();
        Task<ByteStreamResponse<bool>> DataAvailableAsync();
        Task<ByteStreamResponse<int>> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
        Task<ByteStreamResponse<bool>> WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
    }
}
