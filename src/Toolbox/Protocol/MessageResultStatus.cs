using System;

namespace Toolbox.Protocol
{
    public class MessageResultStatus<TMessageStatus> : IMessageResultStatus<TMessageStatus>
        where TMessageStatus : struct
    {
        public TMessageStatus Status { get; protected set; }
        public string Text { get; protected set; }
        public MessageResultStatus(TMessageStatus status, string text = "")
        {
            if (!typeof(TMessageStatus).IsEnum)
            {
                throw new ArgumentException("TMessageStatus must be an enumerated type");
            }
            Status = status;
            Text = text;
        }
    }
}
