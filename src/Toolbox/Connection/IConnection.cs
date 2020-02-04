using System;
using System.Threading;
using System.Threading.Tasks;

namespace Toolbox.Connection
{
    public interface IConnection : IDisposable
    {
        bool Connected { get; }
        int ConnectRetries { get; set; }
        int ConnectTimeout { get; set; }
        bool DataAvaliable { get; }
        int ReceiveRetries { get; set; }
        int ReceiveTimeoutInner { get; set; }
        int ReceiveTimeoutOuter { get; set; }
        int SendRetries { get; set; }
        int SendTimeout { get; set; }
        ConnectionState State { get; }
        IObservable<ConnectionState> StateChanged { get; }

        Task<bool> ConnectAsync();
        Task<bool> DisconnectAsync();
        Task<int> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken);
        Task<bool> SendAsync(byte[] data, CancellationToken cancellationToken);

        Task<byte[]> ReceiveAsync(int bytesToRead, CancellationToken cancellationToken);
        //IObservable<byte> Receive(CancellationToken cancellationToken);
    }
}
