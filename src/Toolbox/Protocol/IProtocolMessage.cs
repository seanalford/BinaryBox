using System;

namespace Toolbox.Protocol
{

    public interface IProtocolMessage<TMessageStatus> : IDisposable
        where TMessageStatus : struct
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
        IProtocolSettings Settings { get; set; }
        TMessageStatus Status { get; }
        int TxBytesToRead { get; }
        byte TxEndOfMessageToken { get; }
        bool ValidateTx { get; }
        bool ValidTx(byte[] data);
    }
}
