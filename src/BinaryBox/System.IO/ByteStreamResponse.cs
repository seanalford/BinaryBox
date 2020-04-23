using BinaryBox.Core.Response;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamResponse<TData> : Response<ByteStreamResponseStatus, ByteStreamResponseStatusCode, TData>, IResponse<ByteStreamResponseStatus, ByteStreamResponseStatusCode, TData>
    {
        public ByteStreamResponse(ByteStreamResponseStatusCode code, TData data = default) : base(code, data)
        {
            Status = new ByteStreamResponseStatus(code);
        }
    }
}
