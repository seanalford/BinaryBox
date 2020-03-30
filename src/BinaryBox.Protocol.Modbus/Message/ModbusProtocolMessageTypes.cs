namespace BinaryBox.Protocol.Modbus
{
    public enum ModbusProtcolMessageTypes
    {
        ReadCoils = 1,
        ReadDiscreteInputs = 2,
        ReadMultipleHoldingRegisters = 3,
        ReadInputRegisters = 4,
        WriteSingleCoil = 5,
        WriteSingleHoldingRegister = 6,
        ReadExceptionStatus = 7,
        Diagnostic = 8,
        GetComEventCounter = 11,
        GetComEventLog = 12,
        WriteMultipleCoils = 15,
        WriteMultipleHoldingRegisters = 16,
        ReportSlaveID = 17,
        ReadFileRecord = 20,
        WriteFileRecord = 21,
        MaskWriteRegister = 22,
        ReadWriteMultipleRegisters = 23,
        ReadFIFOQueue = 24,
        ReadDeviceIdentification = 43,
        //EncapsulatedInterfaceTransport = 43,
    }
}
