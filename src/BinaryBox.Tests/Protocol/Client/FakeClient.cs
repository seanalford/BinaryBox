using BinaryBox.Core.System.IO;
using Microsoft.Extensions.Logging;

namespace BinaryBox.Protocol.Test
{
    public class FakeClient : ProtocolClient<IFakeProtocolSettings, IFakeProtocolMessage<IFakeProtocolSettings, IFakeProtocolMessageData>, IFakeProtocolMessageData>
    {
        public FakeClient(ILogger logger, IByteStreamManager connection, IFakeProtocolSettings settings) : base(logger, connection, settings)
        {

        }

        public override void Dispose()
        {

        }
    }
}
