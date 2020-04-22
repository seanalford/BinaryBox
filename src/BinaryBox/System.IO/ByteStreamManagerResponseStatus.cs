using BinaryBox.Core.Response;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamManagerResponseStatus : ResponseStatus<ByteStreamManagerResponseStatusCode>, IResponseStatus<ByteStreamManagerResponseStatusCode>
    {
        public ByteStreamManagerResponseStatus(ByteStreamManagerResponseStatusCode code) : base(code)
        {
        }

        protected override void Initialize()
        {
            //TODO: Move string literals to english resource file.
            switch (Code)
            {
                case ByteStreamManagerResponseStatusCode.OK:
                    Success = true;
                    Description = "Operation Succesfully";
                    break;
                case ByteStreamManagerResponseStatusCode.Cancelled:
                    Success = false;
                    Description = "Operation Failed: Cancelled";
                    break;
                case ByteStreamManagerResponseStatusCode.OpenCloseTimeout:
                    Success = false;
                    Description = "Operation Failed: Timed Out";
                    break;
                case ByteStreamManagerResponseStatusCode.AlreadyClosed:
                    Success = false;
                    Description = "Operation Failed: Already Closed";
                    break;
                case ByteStreamManagerResponseStatusCode.AlreadyOpen:
                    Success = false;
                    Description = "Operation Failed: Already Open";
                    break;
                case ByteStreamManagerResponseStatusCode.NotOpen:
                    Success = false;
                    Description = "Operation Failed: Not Open";
                    break;
                case ByteStreamManagerResponseStatusCode.WriteTimeout:
                    Success = false;
                    Description = "Operation Failed: Write Timeout";
                    break;
                case ByteStreamManagerResponseStatusCode.PrimaryReadTimeout:
                    Success = false;
                    Description = "Operation Failed: Primary Read Timeout";
                    break;
                case ByteStreamManagerResponseStatusCode.SecondaryReadTimeout:
                    Success = false;
                    Description = "Operation Failed: Secondary Read Timeout";
                    break;
            }
        }
    }

}
