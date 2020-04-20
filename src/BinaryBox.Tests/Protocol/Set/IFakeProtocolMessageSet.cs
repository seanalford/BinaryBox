namespace BinaryBox.Protocol.Test
{
    public interface IFakeProtocolMessageSetItem : IFakeProtocolMessage<IFakeProtocolSettings, IFakeProtocolMessageData>
    {
        IFakeProtocolMessageSetItem Item(int item, float value);
    }
}
