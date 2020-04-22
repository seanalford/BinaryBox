using BinaryBox.Core.Response;

namespace BinaryBox.Protocol
{
    public class ProtocolResponse<TData> : Response<ProtocolResponseStatus, ProtocolResponseStatusCode, TData>, IProtocolResponse<TData>
    {
        public ProtocolResponse(ProtocolResponseStatusCode code, TData data = default) : base(code, data)
        {
            Status = new ProtocolResponseStatus(code);
        }
    }
}
