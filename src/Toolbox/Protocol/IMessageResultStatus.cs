namespace Toolbox.Protocol
{
    public interface IMessageResultStatus<TMessageStatus>
        where TMessageStatus : struct
    {
        TMessageStatus Status { get; }
        string Text { get; }
    }
}
