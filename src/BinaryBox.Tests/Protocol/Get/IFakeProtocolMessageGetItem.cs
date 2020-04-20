namespace BinaryBox.Protocol.Test
{
    public interface IFakeProtocolMessageGetItem : IFakeProtocolMessage<IFakeProtocolSettings, IFakeProtocolMessageData>
    {
        new FakeProtocolMessageGetData Data { get; }
        IFakeProtocolMessageGetItem Item(int item);
    }

}
