using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace BinaryBox.Connection.Test
{
    public class ConnectionMock : Connection
    {
        private readonly IClientStub _client;

        public ConnectionMock(IClientStub client, ILogger logger, IConnectionSettings settings)
            : base(logger, settings)
        {
            _client = client;
        }

        /// <inheritdoc />
        protected override Task<bool> ConnectTask() => _client.Result<bool>();

        /// <inheritdoc />
        public override Task<bool> DataAvailableAsync() => _client.Result<bool>();

        /// <inheritdoc />
        protected override Task<bool> DisconnectTask() => _client.Result<bool>();

        /// <inheritdoc />
        public override void Dispose()
        {
        }

        /// <inheritdoc />
        protected override Task<int> ReadTask(byte[] data, CancellationToken cancellationToken) =>
            _client.Result<int>();

        /// <inheritdoc />
        protected override Task<bool> WriteTask(byte[] data, CancellationToken cancellationToken) =>
            _client.Result<bool>();
    }
}