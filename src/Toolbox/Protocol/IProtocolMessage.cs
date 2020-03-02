using System;

namespace Toolbox.Protocol
{
    public interface IProtocolMessage<TMessageStatus, TMessageData> : IDisposable
        where TMessageStatus : struct
    {
        byte[] Abort { get; }
        byte[] Ack { get; }
        bool Complete { get; }
        TMessageData Data { get; }
        bool Decode(byte[] data);
        byte[] Encode();
        byte[] Nak { get; }
        int RxBytesToRead { get; }
        byte RxEndOfMessageToken { get; }
        TMessageStatus Status { get; }
        int TxBytesToRead { get; }
        byte TxEndOfMessageToken { get; }
        bool ValidateTx { get; }
        bool ValidTx(byte[] data);
    }
}
