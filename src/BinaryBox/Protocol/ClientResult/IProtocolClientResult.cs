namespace BinaryBox.Protocol
{
    public interface IProtocolClientResult<TProtocolMessageData>
    {
        ProtocolClientStatus Status { get; }
        string Description { get; }
        TProtocolMessageData Data { get; }
    }
}