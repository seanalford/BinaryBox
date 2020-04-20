using BinaryBox.Connection;
using Microsoft.Extensions.Logging;

namespace BinaryBox.Protocol.Test
{
    public class FakeClient : ProtocolClient<IFakeProtocolSettings, IFakeProtocolMessage<IFakeProtocolSettings, IFakeProtocolMessageData>, IFakeProtocolMessageData>
    {
        public FakeClient(ILogger logger, IConnection connection, IFakeProtocolSettings settings) : base(logger, connection, settings)
        {

        }

        public override void Dispose()
        {

        }
    }
}
