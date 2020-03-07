namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessageSetItem : IFakeProtocolMessage<FakeProtcolMessageStatus>
    {
        IFakeProtocolMessageSetItem Item(int item, float value);
    }
}
