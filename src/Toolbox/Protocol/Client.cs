using System.Threading;
using System.Threading.Tasks;
using Toolbox.Connection;

namespace Toolbox.Protocol
{
    public abstract class Client<TMethod, TMethodResult, TMethodStatus> : IClient<TMethod, TMethodResult, TMethodStatus>
        where TMethod : IMethod
        where TMethodResult : IMethodResult<TMethodStatus>
        where TMethodStatus : struct
    {
        public IConnection Connection { get; protected set; }

        public Client(IConnection connection)
        {
            Connection = connection;
        }

        public abstract Task<TMethodResult> SendAsync(TMethod method, CancellationToken cancellationToken);
    }
}
