namespace BinaryBox.Protocol.Modbus
{
    public interface IModbusProtocolMessage<TModbusSettings> : IProtocolMessage<TModbusSettings>
        where TModbusSettings : IProtocolSettings
    {

    }
}
