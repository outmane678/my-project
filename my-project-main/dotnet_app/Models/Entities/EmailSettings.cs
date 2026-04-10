namespace dotnet_app;

public class EmailSettings
{
    public string Host { get; set; }
    public int Port { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string From { get; set; }
    public string BackendOrigin { get; set; }
    public bool EnableAuth { get; set; }
    public bool EnableStartTls { get; set; }
    public string SslTrust { get; set; }
}