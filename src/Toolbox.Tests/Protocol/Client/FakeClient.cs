using Toolbox.Connection;

namespace Toolbox.Protocol.Test
{
    public class FakeClient : ProtocolClient<IFakeProtocolMessage<FakeProtcolMessageStatus>, FakeProtcolMessageStatus>
    {
        public FakeClient(IConnection connection, IFakeProtocolSettings settings) : base(connection, settings)
        {

        }

        public override void Dispose()
        {

        }
    }
}
