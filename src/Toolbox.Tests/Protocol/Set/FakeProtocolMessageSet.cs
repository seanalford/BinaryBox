namespace Toolbox.Protocol.Test
{
    public class FakeProtocolMessageSet : FakeProtocolMessage, IFakeProtocolMessageSetItem
    {
        public override void ClearData()
        {
            Data?.Clear();
        }

        public override void DecodeData()
        {

        }

        public FakeProtocolMessageSet(IFakeProtocolSettings protocolSettings) : base(protocolSettings)
        {
            // Disable for testing this use case;
            ValidateTx = false;
        }

        public IFakeProtocolMessageSetItem Item(int item, float value)
        {
            _Type = FakeProtcolMessageTypes.Set;
            _Item = item;
            _Value = value;
            return this;
        }
    }
}
