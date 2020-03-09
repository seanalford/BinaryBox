namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessageSetItem : IFakeProtocolMessage<IFakeProtocolSettings, FakeProtcolMessageStatus>
    {
        IFakeProtocolMessageSetItem Item(int item, float value);
    }
}
