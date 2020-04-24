using BinaryBox.Core.Response;

namespace BinaryBox.Protocol
{
    public interface IProtocolResponse<TData> : IResponse<ProtocolResponseStatusCode, TData>
        where TData : IProtocolMessageData
    {

    }
}
