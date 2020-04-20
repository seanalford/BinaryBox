using BinaryBox.Connection;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Protocol
{
    public interface IProtocolClient<TProtocolSettings, TProtocolMessage, TProtocolMessageData> : IProtocol
        where TProtocolSettings : IProtocolSettings
        where TProtocolMessage : IProtocolMessage<TProtocolSettings, TProtocolMessageData>
        where TProtocolMessageData : IProtocolMessageData
    {
        IConnection Connection { get; }
        Task<IProtocolClientResult<TProtocolMessageData>> SendAsync(TProtocolMessage message, CancellationToken cancellationToken);
    }
}
