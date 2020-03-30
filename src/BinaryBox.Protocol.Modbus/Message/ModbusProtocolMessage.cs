using Microsoft.Extensions.Logging;

namespace BinaryBox.Protocol.Modbus
{
    public abstract class ModbusProtocolMessage : ProtocolMessage<IModbusProtocolSettings>, IModbusProtocolMessage<IModbusProtocolSettings>
    {
        protected byte[] ModbusId { get; private set; }
        protected byte FunctionCode { get; private set; }
        //protected byte[] Data { get; private set; }

        public ModbusProtocolMessage(ILogger logger, IModbusProtocolSettings settings) : base(logger, settings)
        {
            Complete = true;
            RxBytesToRead = 0;
            RxEndOfMessageToken = (byte)MessageTokens.ETX;
            TxBytesToRead = 1;
            TxEndOfMessageToken = 0;
            ValidateTx = true;
        }
    }
}
