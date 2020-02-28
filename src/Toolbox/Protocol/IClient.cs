using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public interface IClient<TMessage, TMessageResult, TMessageResultStatus, TMessageStatus, TMessageResultData> : IDisposable
        where TMessage : IMessage
        where TMessageResult : IMessageResult<TMessageResultStatus, TMessageStatus, TMessageResultData>
        where TMessageResultStatus : IMessageResultStatus<TMessageStatus>
        where TMessageStatus : struct
        where TMessageResultData : IDictionary<string, object>
    {
        IConnection Connection { get; }
        Task<TMessageResult> SendAsync(TMessage message, CancellationToken cancellationToken);
    }
}
