namespace Toolbox.Connection
{
    public interface IConnectionSettings
    {
        Checksum.ChecksumTypes Checksum { get; set; }
        int ConnectRetries { get; set; }
        int ReceiveBufferSize { get; set; }
        int ReceiveRetries { get; set; }
        int ReceiveTimeoutInner { get; set; }
        int ReceiveTimeoutOuter { get; set; }
        int SendBufferSize { get; set; }
        int SendRetries { get; set; }
        int SendTimeout { get; set; }
    }
}