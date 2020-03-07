namespace Toolbox.Protocol.Test
{
    public class FakeProtocolMessageGet : FakeProtocolMessage, IFakeProtocolMessageGetItem
    {
        public new FakeProtocolMessageGetData Data { get; private set; }

        public FakeProtocolMessageGet(IFakeProtocolSettings settings) : base(settings)
        {
            Data = new FakeProtocolMessageGetData();
        }

        public override FakeProtcolMessageStatus DecodeData()
        {
            Data.Item = _Item;
            Data.Value = _Value;
            return FakeProtcolMessageStatus.SUCCESS;
        }

        public IFakeProtocolMessageGetItem Item(int item)
        {
            _Type = FakeProtcolMessageTypes.Get;
            _Item = item;
            return this;
        }
    }
}
