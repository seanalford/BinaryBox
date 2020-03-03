using System;

namespace Toolbox.Protocol
{

    public interface IProtocolMessage : IDisposable
    {
        byte[] Abort { get; }
        byte[] Ack { get; }
        bool Complete { get; }
        IProtocolMessageData Data { get; }
        bool Decode(byte[] data);
        byte[] Encode();
        byte[] Nak { get; }
        int RxBytesToRead { get; }
        byte RxEndOfMessageToken { get; }
        IProtocolMessageStatus Status { get; }
        int TxBytesToRead { get; }
        byte TxEndOfMessageToken { get; }
        bool ValidateTx { get; }
        bool ValidTx(byte[] data);
    }
}
