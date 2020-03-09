namespace Toolbox.Protocol.Test
{
    public interface IFakeProtocolMessage<TProtocolSettings, TMessageStatus> : IProtocolMessage<TProtocolSettings, TMessageStatus>
        where TProtocolSettings : IProtocolSettings
        where TMessageStatus : struct
    {

    }
}
