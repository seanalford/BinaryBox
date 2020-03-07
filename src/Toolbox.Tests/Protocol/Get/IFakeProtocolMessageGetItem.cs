namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessageGetItem : IFakeProtocolMessage<FakeProtcolMessageStatus>
    {
        new FakeProtocolMessageGetData Data { get; }
        IFakeProtocolMessageGetItem Item(int item);
    }

}
