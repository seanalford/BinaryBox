using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public interface IProtocolClient<TProtocolSettings, TMessage, TMessageStatus> : IProtocol
        where TProtocolSettings : IProtocolSettings
        where TMessage : IProtocolMessage<TProtocolSettings, TMessageStatus>
        where TMessageStatus : struct
    {
        IConnection Connection { get; }
        Task<TMessageStatus> SendAsync(TMessage message, CancellationToken cancellationToken);

    }
}
