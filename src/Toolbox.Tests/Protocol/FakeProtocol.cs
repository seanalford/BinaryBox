using Microsoft.Extensions.Logging;
using System;

namespace Toolbox.Protocol.Test
{
    public static class FakeProtocol
    {
        public static Func<ILogger, IFakeProtocolSettings, IFakeProtocolMessageGetItem> Get = (l, s) => { return new FakeProtocolMessageGet(l, s); };
        public static Func<ILogger, IFakeProtocolSettings, IFakeProtocolMessageSetItem> Set = (l, s) => { return new FakeProtocolMessageSet(l, s); };
    }
}
