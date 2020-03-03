using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public interface IProtocolClient<TMessage> : IDisposable
        where TMessage : IProtocolMessage
    {
        IConnection Connection { get; }
        Task<IProtocolMessageStatus> SendAsync(TMessage message, CancellationToken cancellationToken);

    }
}
