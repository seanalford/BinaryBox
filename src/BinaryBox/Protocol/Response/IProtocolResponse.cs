using BinaryBox.Core.Response;

namespace BinaryBox.Protocol
{
    public interface IProtocolResponse<TData> : IResponse<ProtocolResponseStatus, ProtocolResponseStatusCode, TData>
    {

    }

}
