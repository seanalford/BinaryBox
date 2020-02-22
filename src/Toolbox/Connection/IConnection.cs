using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Connection
{
    public interface IConnection : IDisposable
    {
        IConnectionSettings Settings { get; set; }
        ConnectionState State { get; }
        Task<ConnectionState> ConnectAsync();
        Task<bool> DataAvailableAsync();
        Task<ConnectionState> DisconnectAsync();
        Task<byte[]> ReadAsync(int bytesToRead, CancellationToken cancellationToken);
        Task<bool> WriteAsync(byte[] data, CancellationToken cancellationToken);
    }
}