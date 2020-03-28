using Microsoft.Extensions.Logging;

namespace BinaryBox.Protocol.Modbus
{
    public abstract class ModbusProtocolMessage : ProtocolMessage<IModbusProtocolSettings>, IModbusProtocolMessage<IModbusProtocolSettings>
    {
        public ModbusProtocolMessage(ILogger logger, IModbusProtocolSettings settings) : base(logger, settings)
        {

            // Why isn't the compiler compaining about not implementing the abstract messages in ProtocolMessage?

        }
    }
}
