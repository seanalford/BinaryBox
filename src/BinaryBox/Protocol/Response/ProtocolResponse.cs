using BinaryBox.Core.Response;
using BinaryBox.Protocol

namespace BinaryBox.Core.System.IO
{
    public class ProtocolResponse<TData> : IResponse<ProtocolResponseStatus, ProtocolResponseStatusCode, TData>
    {
        public ProtocolResponseStatus Status { get; }

        public TData Data { get; }

        public ProtocolResponse(ProtocolResponseStatusCode code, TData data)
        {
            Status = new ProtocolResponseStatus(code);
            Data = data;
        }
    }

}
