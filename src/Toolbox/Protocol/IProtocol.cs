using Microsoft.Extensions.Logging;
using System;

namespace BinaryBox.Protocol
{
    public interface IProtocol : IDisposable
    {
        ILogger Log { get; }
    }
}
