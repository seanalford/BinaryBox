using Microsoft.Extensions.Logging;
using System;

namespace Toolbox.Protocol
{
    public interface IProtocol : IDisposable
    {
        ILogger Log { get; }
    }
}
