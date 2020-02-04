using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public interface IClient<TMethod, TMethodResult, TMethodResultStatus, TMethodStatus, TMethodResultData>
        where TMethod : IMethod
        where TMethodResult : IMethodResult<TMethodResultStatus, TMethodStatus, TMethodResultData>
        where TMethodResultStatus : IMethodResultStatus<TMethodStatus>
        where TMethodStatus : struct
        where TMethodResultData : IDictionary<string, object>
    {
        IConnection Connection { get; }
        Task<TMethodResult> SendAsync(TMethod method, CancellationToken cancellationToken);
    }

    public abstract class Client<TMethod, TMethodResult, TMethodResultStatus, TMethodStatus, TMethodResultData> : IClient<TMethod, TMethodResult, TMethodResultStatus, TMethodStatus, TMethodResultData>
        where TMethod : IMethod
        where TMethodResult : IMethodResult<TMethodResultStatus, TMethodStatus, TMethodResultData>
        where TMethodResultStatus : IMethodResultStatus<TMethodStatus>
        where TMethodStatus : struct
        where TMethodResultData : IDictionary<string, object>
    {
        public IConnection Connection { get; protected set; }

        public Client(IConnection connection)
        {
            Connection = connection;
        }
        public abstract Task<TMethodResult> SendAsync(TMethod method, CancellationToken cancellationToken);
    }
}
