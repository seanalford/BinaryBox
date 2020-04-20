using Microsoft.Extensions.Logging;

namespace BinaryBox.Protocol
{
    public abstract class ProtocolMessage<TProtocolSettings, TProtocolMessageData> : Protocol, IProtocolMessage<TProtocolSettings, TProtocolMessageData>
        where TProtocolSettings : IProtocolSettings
        where TProtocolMessageData : IProtocolMessageData
    {
        public ProtocolMessage(ILogger logger, TProtocolSettings settings) : base(logger)
        {
            Settings = settings;
        }
        public byte[] Abort { get; protected set; }

        public byte[] Ack { get; protected set; }

        public abstract void ClearData();

        public bool Complete { get; protected set; }

        public TProtocolMessageData Data { get; protected set; }

        public abstract bool Decode(byte[] data);

        public abstract void DecodeData();

        public abstract byte[] Encode();

        public byte[] Nak { get; protected set; }

        public int RxBytesToRead { get; protected set; }

        public byte RxEndOfMessageToken { get; protected set; }
        public TProtocolSettings Settings { get; set; }

        public int TxBytesToRead { get; protected set; }

        public byte TxEndOfMessageToken { get; protected set; }

        public bool ValidateTx { get; protected set; }

        public abstract bool ValidTx(byte[] data);
    }
}
