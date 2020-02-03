using System;

namespace Toolbox.Communication.Protocol
{
    public interface IMethodResult<TStatus> where TStatus : struct
    {
        TStatus Status { get; }
        String Text { get; }
    }
}
