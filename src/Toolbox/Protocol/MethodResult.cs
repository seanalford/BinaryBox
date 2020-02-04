using System.Collections.Generic;

namespace Toolbox.Protocol
{
    public interface IMethodResult<TMethodResultStatus, TMethodStatus, TMethodResultData>
        where TMethodResultStatus : IMethodResultStatus<TMethodStatus>
        where TMethodStatus : struct
        where TMethodResultData : IDictionary<string, object>
    {
        TMethodResultStatus Status { get; }
        TMethodResultData Data { get; }
    }

    public abstract class MethodResult<TMethodResultStatus, TMethodStatus, TMethodResultData> : IMethodResult<TMethodResultStatus, TMethodStatus, TMethodResultData>
        where TMethodResultStatus : IMethodResultStatus<TMethodStatus>
        where TMethodStatus : struct
        where TMethodResultData : IDictionary<string, object>
    {
        public TMethodResultStatus Status { get; protected set; }

        public TMethodResultData Data { get; protected set; }

        public MethodResult(TMethodResultStatus status, TMethodResultData data)
        {
            Status = status;
            Data = data;
        }
    }
}
