using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
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
