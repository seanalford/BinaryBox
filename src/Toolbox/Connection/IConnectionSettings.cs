namespace Toolbox.Connection
{
    public interface IConnectionSettings
    {
        int ReceiveBufferSize { get; set; }
        int ReceiveTimeoutInner { get; set; }
        int ReceiveTimeoutOuter { get; set; }
        int SendBufferSize { get; set; }
        int SendTimeout { get; set; }
    }
}