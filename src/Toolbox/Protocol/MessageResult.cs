using System.Collections.Generic;

namespace Toolbox.Protocol
{
    public abstract class MessageResult<TMessageResultStatus, TMessageStatus, TMessageResultData> : IMessageResult<TMessageResultStatus, TMessageStatus, TMessageResultData>
        where TMessageResultStatus : IMessageResultStatus<TMessageStatus>
        where TMessageStatus : struct
        where TMessageResultData : IDictionary<string, object>
    {
        public TMessageResultStatus Status { get; protected set; }

        public TMessageResultData Data { get; protected set; }

        public MessageResult(TMessageResultStatus status, TMessageResultData data)
        {
            Status = status;
            Data = data;
        }
    }
}
