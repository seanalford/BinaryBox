using BinaryBox.Connection;
using Microsoft.Extensions.Logging;

namespace BinaryBox.Protocol.Modbus
{
    public class ModbusClient : ProtocolClient<IModbusProtocolSettings, IModbusProtocolMessage<IModbusProtocolSettings>>
    {
        public ModbusClient(ILogger logger, IConnection connection, IModbusProtocolSettings settings) : base(logger, connection, settings)
        {

        }
        public override void Dispose()
        {

        }
    }
}
