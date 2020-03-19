using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public interface IProtocolClient<TProtocolSettings, TMessage> : IProtocol
        where TProtocolSettings : IProtocolSettings
        where TMessage : IProtocolMessage<TProtocolSettings>
    {
        IConnection Connection { get; }
        Task<IProtocolClientStatus> SendAsync(TMessage message, CancellationToken cancellationToken);
    }
}
