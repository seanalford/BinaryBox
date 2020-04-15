namespace BinaryBox.Protocol
{
    public class ProtocolClientResult : IProtocolClientResult
    {
        public ProtocolClientResults Result { get; }
        public string Description { get; }

        public ProtocolClientResult(ProtocolClientResults code)
        {
            Result = code;

        }
    }
}
