namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessageGetData : IFakeProtocolMessageData
    {
        int Item { get; set; }
        float Value { get; set; }
    }
}
