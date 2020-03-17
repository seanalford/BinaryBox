using Microsoft.Extensions.Logging;

namespace Toolbox.Protocol.Test
{
    public class FakeProtocolMessageGet : FakeProtocolMessage, IFakeProtocolMessageGetItem
    {
        public override void ClearData()
        {
            Data?.Clear();
        }
        public new FakeProtocolMessageGetData Data { get; private set; }

        public FakeProtocolMessageGet(ILogger logger, IFakeProtocolSettings settings) : base(logger, settings)
        {
            Data = new FakeProtocolMessageGetData();
        }

        public override void DecodeData()
        {
            Data.Item = _Item;
            Data.Value = _Value;
        }

        public IFakeProtocolMessageGetItem Item(int item)
        {
            _Type = FakeProtcolMessageTypes.Get;
            _Item = item;
            return this;
        }
    }
}
