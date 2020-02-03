using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Toolbox.Communication.Connection
{
    public abstract class Connection : IConnection
    {
        private const int DEFAULT_CONNECT_RETIRES = 3000;
        private const int DEFAULT_CONNECT_TIMEOUT = 15000;
        private const int DEFAULT_SEND_RETIRES = 3000;
        private const int DEFAULT_SEND_TIMEOUT = 15000;
        private const int DEFAULT_RECEIVE_RETRIES = 3000;
        private const int DEFAULT_RECEIVE_TIMEOUT_INNER = 1000;
        private const int DEFAULT_RECEIVE_TIMEOUT_OUTER = 15000;

        [Reactive] public int ConnectRetries { get; set; } = DEFAULT_CONNECT_RETIRES;
        [Reactive] public int ConnectTimeout { get; set; } = DEFAULT_CONNECT_TIMEOUT;
        [Reactive] public int SendRetries { get; set; } = DEFAULT_SEND_RETIRES;
        [Reactive] public int SendTimeout { get; set; } = DEFAULT_SEND_TIMEOUT;
        [Reactive] public int ReceiveRetries { get; set; } = DEFAULT_RECEIVE_RETRIES;
        [Reactive] public int ReceiveTimeoutInner { get; set; } = DEFAULT_RECEIVE_TIMEOUT_INNER;
        [Reactive] public int ReceiveTimeoutOuter { get; set; } = DEFAULT_RECEIVE_TIMEOUT_OUTER;
        [Reactive] public bool Connected { get; protected set; } = false;
        [Reactive] public bool DataAvaliable { get; protected set; } = false;
        [Reactive] public ConnectionState State { get; protected set; } = ConnectionState.Disconnected;
        public IObservable<ConnectionState> StateChanged { get; protected set; }                

        public Connection()
        {
            StateChanged = this.WhenAnyValue(x => x.State).DistinctUntilChanged();            
        }

        public abstract Task<bool> ConnectAsync();
        public abstract Task<bool> DisconnectAsync();
        public abstract void Dispose();
        public abstract Task<int> ReceiveAsync(byte[] buffer, CancellationToken cancellationToken);
        public abstract Task<bool> SendAsync(byte[] data, CancellationToken cancellationToken);
        public abstract Task<byte[]> ReceiveAsync(int bytesToRead, CancellationToken cancellationToken);
        

        //public IObservable<byte> Receive(CancellationToken cancellationToken)
        //{
        //    return Observable.Create<byte>(d =>
        //    {
        //        IDisposable result = null;
        //        byte[] buffer = new byte[1024];
        //        try
        //        {
        //            while (State == ConnectionState.Conneted)
        //            {
        //                while (DataAvaliable)
        //                {
        //                    result = ReceiveAsync(buffer, cancellationToken)
        //                    .ToObservable()
        //                    .Where(x => x > 0)
        //                    .Subscribe(_ =>
        //                    {
        //                        foreach (byte item in buffer)
        //                        {
        //                            d.OnNext(item);
        //                        }
        //                    });
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            d.OnError(ex);
        //        }
        //        d.OnCompleted();
        //        return result;
        //    });
        //}
    }
}
