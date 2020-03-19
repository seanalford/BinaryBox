namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessageSetItem : IFakeProtocolMessage<IFakeProtocolSettings>
    {
        IFakeProtocolMessageSetItem Item(int item, float value);
    }
}
