using System.Threading;
using System.Threading.Tasks;
using Toolbox.Communication.Connection;

namespace Toolbox.Communication.Protocol
{
    public interface IClient<TMethod, TMethodResult, TMethodStatus>
        where TMethod : IMethod
        where TMethodResult : IMethodResult<TMethodStatus>
        where TMethodStatus : struct // Must be an enum
    {
        IConnection Connection { get; }
        Task<TMethodResult> SendAsync(TMethod method, CancellationToken cancellationToken);
    }
}
