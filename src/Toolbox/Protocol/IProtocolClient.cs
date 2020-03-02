using System;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public interface IProtocolClient<TMessage, TMessageStatus, TMessageData> : IDisposable
        where TMessage : IProtocolMessage<TMessageStatus, TMessageData>
        where TMessageStatus : struct
    {
        IConnection Connection { get; }
        Task<TMessageStatus> SendAsync(TMessage message, CancellationToken cancellationToken);

    }
}
