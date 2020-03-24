using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Connection.Test
{
    public partial class TestConnection
    {
        public class ConnectionFake : Connection
        {
            private HostFake Host;
            public int TaskDelay = 0;
            public bool ExpectedResult = false;

            public ConnectionFake(ILogger logger, IConnectionSettings settings) : base(logger, settings)
            {
                Host = new HostFake();
            }

            public override async Task<bool> DataAvailableAsync()
            {
                return await Host.DataAvaliable();
            }

            public override void Dispose()
            {
                Host.Dispose();
            }

            protected async override Task<bool> ConnectTask()
            {
                return await Host.ConnectAsync(TaskDelay, ExpectedResult);
            }

            protected async override Task<bool> DisconnectTask()
            {
                return await Host.DisconnectAsync(TaskDelay, ExpectedResult);
            }

            protected async override Task<int> ReadTask(byte[] data, CancellationToken cancellationToken)
            {
                return await Host.ReadAsync(data, cancellationToken);
            }

            protected async override Task<bool> WriteTask(byte[] data, CancellationToken cancellationToken)
            {
                return await Host.WriteAsync(data, cancellationToken);
            }

            public async Task WriteToRxBuffer(byte[] data, int delayPerByte = 0)
            {
                await Host.WriteToInputRxBuffer(data, delayPerByte);
            }

        }
    }
}
