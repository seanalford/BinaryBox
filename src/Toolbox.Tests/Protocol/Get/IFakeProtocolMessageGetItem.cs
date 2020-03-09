namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessageGetItem : IFakeProtocolMessage<IFakeProtocolSettings, FakeProtcolMessageStatus>
    {
        new FakeProtocolMessageGetData Data { get; }
        IFakeProtocolMessageGetItem Item(int item);
    }

}
