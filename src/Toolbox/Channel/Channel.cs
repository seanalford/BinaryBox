//using System;
//using System.Reactive.Subjects;
//using System.Threading;
//using System.Threading.Tasks;
//using Toolbox.Connection;

//namespace Toolbox.Channel
//{
//    public interface IEavesdrop
//    {
//        IChannel Channel { get; }
//        Subject<byte> Data { get; }
//    }

//    public interface IChannel : IDisposable
//    {
//        IConnection Connection { get; }
//        Task Write(byte[] data, CancellationToken cancellationToken);
//        ISubject<byte[]> Tx { get; }
//        ISubject<byte[]> Rx { get; }
//    }

//    public class Channel : IChannel
//    {
//        public IConnection Connection { get; private set; }
//        public ISubject<byte[]> Tx { get; private set; }
//        public ISubject<byte[]> Rx { get; private set; }

//        public Channel(IConnection connection)
//        {
//            Connection = connection;
//            Tx = new Subject<byte[]>();
//            Rx = new Subject<byte[]>();
//        }

//        public async Task RxAsync()
//        {
//            while (true)
//            {
//                while (Connection.DataAvaliable)
//                {

//                }
//            }
//        }

//        public void Dispose()
//        {
//            Rx?.OnCompleted();
//            Tx?.OnCompleted();
//        }

//        public async Task Write(byte[] data, CancellationToken cancellationToken)
//        {
//            try
//            {
//                Tx?.OnNext(data);
//                await Connection.WriteAsync(data, 0, data.Length, cancellationToken);
//            }
//            catch (Exception ex)
//            {
//                Tx?.OnError(ex);
//            }
//        }
//    }
//}
