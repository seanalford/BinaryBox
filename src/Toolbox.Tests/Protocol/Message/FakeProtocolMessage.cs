using System;
using System.Linq;
using System.Text;
using Toolbox.Checksum;
using Toolbox.IEEE;

namespace Toolbox.Protocol.Test
{
    public abstract class FakeProtocolMessage : ProtocolMessage<IFakeProtocolSettings, FakeProtcolMessageStatus>, IFakeProtocolMessage<IFakeProtocolSettings, FakeProtcolMessageStatus>
    {
        protected FakeProtcolMessageTypes _Type;
        protected int _Item;
        protected float _Value;

        public FakeProtocolMessage(IFakeProtocolSettings settings) : base(settings)
        {
            Abort = BitConverter.GetBytes(MessageTokens.ESC);
            Ack = BitConverter.GetBytes(MessageTokens.ACK);
            Complete = true;
            Nak = BitConverter.GetBytes(MessageTokens.NAK);
            RxBytesToRead = 0;
            RxEndOfMessageToken = (byte)MessageTokens.ETX;
            Status = FakeProtcolMessageStatus.FAIL;
            TxBytesToRead = 1;
            TxEndOfMessageToken = 0;
            ValidateTx = true;
        }

        public override bool Decode(byte[] data)
        {
            Status = FakeProtcolMessageStatus.FAIL;

            if (DecodeMessage(data))
            {
                Status = FakeProtcolMessageStatus.SUCCESS;
            }

            return Status == FakeProtcolMessageStatus.SUCCESS;
        }

        public virtual bool DecodeMessage(byte[] data)
        {
            if ((data.Length == 0) ||
                        (data[0] != MessageTokens.STX) ||
                        (data[data.Length - Settings.Checksum.Length() - 1] != MessageTokens.ETX))
            {
                return false;
            }

            int messageLength = data.Length - 2 - Settings.Checksum.Length();
            byte[] message = new byte[messageLength];
            Array.Copy(data, 1, message, 0, messageLength);

            byte[] checksum = new byte[Settings.Checksum.Length()];
            int checksumIndex = data.Length - Settings.Checksum.Length();
            Array.Copy(data, checksumIndex, checksum, 0, Settings.Checksum.Length());

            if (Settings.Checksum != ChecksumTypes.None)
            {
                if (!message.Checksum(Settings.Checksum).SequenceEqual(checksum)) return false;
            }

            _Type = (FakeProtcolMessageTypes)Convert.ToByte(Encoding.ASCII.GetString(message, 0, 2), 16);
            _Item = Convert.ToInt32(Encoding.ASCII.GetString(message, 2, 4), 16);
            _Value = Encoding.ASCII.GetString(message, 6, 8).ToFloat();

            return true;

        }

        public override void Dispose() { }

        public override byte[] Encode()
        {
            string message = String.Format("{0:X2}{1:X4}{2:x8}", (byte)_Type, _Item, _Value.ToIEEEString());
            byte[] checksum = Encoding.ASCII.GetBytes(message).Checksum(Settings.Checksum);
            string messageTx = String.Format("{0}{1}{2}", MessageTokens.STX, message, MessageTokens.ETX);
            byte[] result = Encoding.ASCII.GetBytes(messageTx);
            if (Settings.Checksum != ChecksumTypes.None)
            {
                Array.Resize(ref result, result.Length + Settings.Checksum.Length());
                Array.Copy(checksum, 0, result, result.Length - checksum.Length, checksum.Length);
            }
            return result;
        }

        public override bool ValidTx(byte[] data)
        {
            // TODO: data.StartsWith(Ack);
            return data[0] == Ack[0];
        }
    };
}
