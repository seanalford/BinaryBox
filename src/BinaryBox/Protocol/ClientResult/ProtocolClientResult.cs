namespace BinaryBox.Protocol
{
    public class ProtocolClientResult<TProtocolMessageData> : IProtocolClientResult<TProtocolMessageData>
        where TProtocolMessageData : IProtocolMessageData
    {
        public ProtocolClientStatus Status { get; }
        public string Description { get; }
        public TProtocolMessageData Data { get; }

        public ProtocolClientResult(ProtocolClientStatus result, TProtocolMessageData data = default)
        {
            Status = result;
            Data = data;
        }
    }
}
