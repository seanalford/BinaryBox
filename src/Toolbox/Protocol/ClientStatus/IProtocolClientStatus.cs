namespace Toolbox.Protocol
{
    public interface IProtocolClientStatus
    {
        ProtocolClientStatusCodes Status { get; }
        string Description { get; }
    }
}