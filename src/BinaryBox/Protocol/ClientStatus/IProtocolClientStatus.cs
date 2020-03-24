namespace BinaryBox.Protocol
{
    public interface IProtocolClientStatus
    {
        ProtocolClientStatusCodes Status { get; }
        string Description { get; }
    }
}