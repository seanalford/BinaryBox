using BinaryBox.Core.Response;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamResponseStatus : ResponseStatus<ByteStreamResponseStatusCode>, IResponseStatus<ByteStreamResponseStatusCode>
    {
        public ByteStreamResponseStatus(ByteStreamResponseStatusCode code) : base(code)
        {
        }

        protected override void Initialize()
        {
            //TODO: Move string literals to english resource file.
            switch (Code)
            {
                case ByteStreamResponseStatusCode.OK:
                    Success = true;
                    Description = "Operation Succesfully";
                    break;
                case ByteStreamResponseStatusCode.Cancelled:
                    Success = false;
                    Description = "Operation Failed: Cancelled";
                    break;
                case ByteStreamResponseStatusCode.OpenCloseTimeout:
                    Success = false;
                    Description = "Operation Failed: Timed Out";
                    break;
                case ByteStreamResponseStatusCode.AlreadyClosed:
                    Success = false;
                    Description = "Operation Failed: Already Closed";
                    break;
                case ByteStreamResponseStatusCode.AlreadyOpen:
                    Success = false;
                    Description = "Operation Failed: Already Open";
                    break;
                case ByteStreamResponseStatusCode.NotOpen:
                    Success = false;
                    Description = "Operation Failed: Not Open";
                    break;
                case ByteStreamResponseStatusCode.WriteTimeout:
                    Success = false;
                    Description = "Operation Failed: Write Timeout";
                    break;
                case ByteStreamResponseStatusCode.PrimaryReadTimeout:
                    Success = false;
                    Description = "Operation Failed: Primary Read Timeout";
                    break;
                case ByteStreamResponseStatusCode.SecondaryReadTimeout:
                    Success = false;
                    Description = "Operation Failed: Secondary Read Timeout";
                    break;
            }
        }
    }

}
