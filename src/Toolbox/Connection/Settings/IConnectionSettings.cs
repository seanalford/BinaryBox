namespace Toolbox.Connection
{
    public interface IConnectionSettings
    {
        int ReadBufferSize { get; set; }
        int PrimaryReadTimeout { get; set; }
        int SecondaryReadTimeout { get; set; }
        int WriteBufferSize { get; set; }
        int WriteTimeout { get; set; }
    }
}