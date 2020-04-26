using BinaryBox.Core.System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Protocol
{
    public interface IProtocolClient<TProtocolSettings, TProtocolMessage, TProtocolMessageData> : IProtocol
        where TProtocolSettings : IProtocolSettings
        where TProtocolMessage : IProtocolMessage<TProtocolSettings, TProtocolMessageData>
        where TProtocolMessageData : IProtocolMessageData
    {
        IByteStreamManager Connection { get; }
        //IConnection Connection { get; }
        Task<IProtocolResponse<TProtocolMessageData>> SendAsync(TProtocolMessage message, CancellationToken cancellationToken);
    }
}
