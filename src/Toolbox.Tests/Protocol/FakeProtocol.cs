using System;

namespace Toolbox.Protocol.Test
{
    public static class FakeProtocol
    {
        public static Func<IFakeProtocolSettings, IFakeProtocolMessageGetItem> Get = (s) => { return new FakeProtocolMessageGet(s); };
        public static Func<IFakeProtocolSettings, IFakeProtocolMessageSetItem> Set = (s) => { return new FakeProtocolMessageSet(s); };
    }
}
