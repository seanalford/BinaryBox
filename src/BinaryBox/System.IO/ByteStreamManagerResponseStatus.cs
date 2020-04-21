using BinaryBox.Core.Response;

namespace BinaryBox.Core.System.IO
{
    public class ByteStreamManagerResponseStatus : IResponseStatus<ByteStreamManagerResponseStatusCode>
    {
        public ByteStreamManagerResponseStatusCode Code { get; }

        public string Description { get; }

        public bool Success { get; }

        public ByteStreamManagerResponseStatus(ByteStreamManagerResponseStatusCode code)
        {
            //TODO: Move string literals to english resource file.
            Code = code;
            switch (Code)
            {
                case ByteStreamManagerResponseStatusCode.OK:
                    Success = true;
                    Description = "Operation Succesfully";
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
