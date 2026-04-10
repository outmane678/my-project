namespace dotnet_app.Services;

public interface IEmailSender
{
    void SendAccountCreationEmail(string to, string token);
}
