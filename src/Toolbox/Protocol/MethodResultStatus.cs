using System;

namespace Toolbox.Protocol
{
    public interface IMethodResultStatus<TMethodStatus>
        where TMethodStatus : struct
    {
        TMethodStatus Status { get; }
        string Text { get; }
    }

    public class MethodResultStatus<TMethodStatus> : IMethodResultStatus<TMethodStatus>
        where TMethodStatus : struct
    {
        public TMethodStatus Status { get; protected set; }
        public string Text { get; protected set; }
        public MethodResultStatus(TMethodStatus status, string text = "")
        {
            if (!typeof(TMethodStatus).IsEnum)
            {
                throw new ArgumentException("TMethodStatus must be an enumerated type");
            }
            Status = status;
            Text = text;
        }
    }
}
