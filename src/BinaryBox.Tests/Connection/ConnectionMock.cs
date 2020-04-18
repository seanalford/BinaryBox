using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BinaryBox.Connection.Test
{
    public class ConnectionMock : Connection
    {
        public IDummyClient Client { get; }

        public ConnectionMock(IDummyClient client, ILogger logger, IConnectionSettings settings) : base(logger, settings)
        {
            Client = client;
        }

        protected override Task<bool> ConnectTask() => Client.Return<bool>();

        public override Task<bool> DataAvailableAsync()
        {
            throw new System.NotImplementedException();
        }

        protected override Task<bool> DisconnectTask()
        {
            throw new System.NotImplementedException();
        }

        public override void Dispose()
        {
            throw new System.NotImplementedException();
        }

        protected override Task<int> ReadTask(byte[] data, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }

        protected override Task<bool> WriteTask(byte[] data, CancellationToken cancellationToken)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface IDummyClient
    {
        Task Return();

        Task<T> Return<T>();
    }
}