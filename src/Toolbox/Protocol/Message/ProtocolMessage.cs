using Microsoft.Extensions.Logging;

namespace Toolbox.Protocol
{
    public abstract class ProtocolMessage<TProtocolSettings> : Protocol, IProtocolMessage<TProtocolSettings>
        where TProtocolSettings : IProtocolSettings
    {
        public ProtocolMessage(ILogger logger, TProtocolSettings settings) : base(logger)
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
