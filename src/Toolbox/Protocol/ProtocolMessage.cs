namespace Toolbox.Protocol
{
    public abstract class ProtocolMessage<TProtocolSettings, TMessageStatus> : IProtocolMessage<TProtocolSettings, TMessageStatus>
        where TProtocolSettings : IProtocolSettings
        where TMessageStatus : struct
    {
        public ProtocolMessage(TProtocolSettings settings)
        {
            Settings = settings;
        }
        public byte[] Abort { get; protected set; }

        public byte[] Ack { get; protected set; }

        public abstract void ClearData();

        public bool Complete { get; protected set; }

        public IProtocolMessageData Data { get; protected set; }

        public abstract bool Decode(byte[] data);

        public abstract void DecodeData();

        public abstract void Dispose();

        public abstract byte[] Encode();

        public byte[] Nak { get; protected set; }

        public int RxBytesToRead { get; protected set; }

        public byte RxEndOfMessageToken { get; protected set; }
        public TProtocolSettings Settings { get; set; }

        public TMessageStatus Status { get; protected set; }

        public int TxBytesToRead { get; protected set; }

        public byte TxEndOfMessageToken { get; protected set; }

        public bool ValidateTx { get; protected set; }

        public abstract bool ValidTx(byte[] data);
    }
}
