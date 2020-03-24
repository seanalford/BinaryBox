namespace BinaryBox.Protocol.Test
{
    public interface IFakeProtocolMessageGetItem : IFakeProtocolMessage<IFakeProtocolSettings>
    {
        new FakeProtocolMessageGetData Data { get; }
        IFakeProtocolMessageGetItem Item(int item);
    }

}
