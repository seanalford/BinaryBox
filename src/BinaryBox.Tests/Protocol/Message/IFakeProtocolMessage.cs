using BinaryBox.Protocol.Settings;

namespace BinaryBox.Protocol.Test
{
    public interface IFakeProtocolMessage<TProtocolSettings, TProtocolMessageData> : IProtocolMessage<TProtocolSettings, TProtocolMessageData>
        where TProtocolSettings : IProtocolSettings
        where TProtocolMessageData : IProtocolMessageData
    {

    }
}
