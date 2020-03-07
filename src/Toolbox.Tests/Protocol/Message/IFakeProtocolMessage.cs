namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessage<TMessageStatus> : IProtocolMessage<TMessageStatus>
        where TMessageStatus : struct
    {

    }
}
