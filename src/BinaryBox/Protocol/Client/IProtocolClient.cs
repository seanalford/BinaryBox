using BinaryBox.Connection;
using System.Threading;
using System.Threading.Tasks;

namespace BinaryBox.Protocol
{
    public interface IProtocolClient<TProtocolSettings, TMessage> : IProtocol
        where TProtocolSettings : IProtocolSettings
        where TMessage : IProtocolMessage<TProtocolSettings>
    {
        IConnection Connection { get; }
        Task<IProtocolClientResult> SendAsync(TMessage message, CancellationToken cancellationToken);
    }
}
