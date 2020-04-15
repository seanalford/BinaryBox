namespace BinaryBox.Protocol
{
    public interface IProtocolClientResult
    {
        ProtocolClientResults Result { get; }
        string Description { get; }
    }
}