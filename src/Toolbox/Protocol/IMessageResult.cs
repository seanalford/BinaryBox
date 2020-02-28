using System.Collections.Generic;

namespace Toolbox.Protocol
{
    public interface IMessageResult<TMessageResultStatus, TMessageStatus, TMessageResultData>
        where TMessageResultStatus : IMessageResultStatus<TMessageStatus>
        where TMessageStatus : struct
        where TMessageResultData : IDictionary<string, object>
    {
        TMessageResultStatus Status { get; }
        TMessageResultData Data { get; }
    }
}
