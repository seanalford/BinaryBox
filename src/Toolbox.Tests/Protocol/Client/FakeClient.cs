using Toolbox.Connection;

namespace Toolbox.Protocol.Test
{
    public class FakeClient : ProtocolClient<IFakeProtocolSettings, IFakeProtocolMessage<IFakeProtocolSettings, FakeProtcolMessageStatus>, FakeProtcolMessageStatus>
    {
        public FakeClient(IConnection connection, IFakeProtocolSettings settings) : base(connection, settings)
        {

        }

        public override void Dispose()
        {

        }
    }
}
