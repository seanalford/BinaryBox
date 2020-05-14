using BinaryBox.Protocol.Settings;

namespace BinaryBox.Protocol
{
    public interface IProtocolMessage<TProtocolSettings, TProtocolMessageData> : IProtocol
        where TProtocolSettings : IProtocolSettings
        where TProtocolMessageData : IProtocolMessageData
    {
        byte[] Abort { get; }
        byte[] Ack { get; }
        void ClearData();
        bool Complete { get; }
        TProtocolMessageData Data { get; }
        bool Decode(byte[] data);
        void DecodeData();
        byte[] Encode();
        byte[] Nak { get; }
        int RxBytesToRead { get; }
        byte RxEndOfMessageToken { get; }
        TProtocolSettings Settings { get; set; }
        int TxBytesToRead { get; }
        byte TxEndOfMessageToken { get; }
        bool ValidateTx { get; }
        bool ValidTx(byte[] data);
    }
}
