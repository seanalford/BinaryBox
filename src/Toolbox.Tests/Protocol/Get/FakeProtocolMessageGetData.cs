namespace Toolbox.Protocol.Test
{
    public class FakeProtocolMessageGetData : FakeProtocolMessageData, IFakeProtocolMessageGetData
    {
        public int Item { get; set; }
        public float Value { get; set; }

        public new void Clear()
        {
            base.Clear();
            Item = default;
            Value = default;
        }
    }
}
