using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public abstract class Client<TMessage, TMessageResult, TMessageResultStatus, TMessageStatus, TMessageResultData> : IClient<TMessage, TMessageResult, TMessageResultStatus, TMessageStatus, TMessageResultData>
        where TMessage : IMessage
        where TMessageResult : IMessageResult<TMessageResultStatus, TMessageStatus, TMessageResultData>
        where TMessageResultStatus : IMessageResultStatus<TMessageStatus>
        where TMessageStatus : struct
        where TMessageResultData : IDictionary<string, object>
    {
        public IConnection Connection { get; protected set; }

        public Client(IConnection connection)
        {
            Connection = connection;
        }
        public abstract Task<TMessageResult> SendAsync(TMessage message, CancellationToken cancellationToken);

        public abstract void Dispose();

    }
}
