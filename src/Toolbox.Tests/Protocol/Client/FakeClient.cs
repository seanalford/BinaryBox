using Microsoft.Extensions.Logging;
using Toolbox.Connection;

namespace Toolbox.Protocol.Test
{
    public class FakeClient : ProtocolClient<IFakeProtocolSettings, IFakeProtocolMessage<IFakeProtocolSettings>>
    {
        public FakeClient(ILogger logger, IConnection connection, IFakeProtocolSettings settings) : base(logger, connection, settings)
        {

        }

        public override void Dispose()
        {

        }
    }
}
