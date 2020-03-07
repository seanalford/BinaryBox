namespace Toolbox.Protocol.Test
{
    public class FakeProtocolMessageSet : FakeProtocolMessage, IFakeProtocolMessageSetItem
    {

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
