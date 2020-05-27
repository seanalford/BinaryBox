using BinaryBox.Protocol.Settings;
using Microsoft.Extensions.Logging;
using System;

namespace BinaryBox.Core.ProtocolV2
{
    public interface IProtocol : IDisposable
    {
        ILogger Log { get; }
        IProtocolSettings Settings { get; }
    }
}
