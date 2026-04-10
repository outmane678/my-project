using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace dotnet_app.Services;

public class EmailService
{
    // PLUS DE CHAMP _smtpClient - On crée une nouvelle connexion à chaque envoi
    private readonly string ORIGIN;
    private readonly string from;
    private readonly string _password;
    private readonly string _host;
    private readonly int _port;

    public EmailService(IConfiguration configuration)
    {
        ORIGIN = configuration["EmailSettings:BackendOrigin"] 
            ?? throw new InvalidOperationException("EmailSettings:BackendOrigin not configured");
        
        from = configuration["EmailSettings:Username"]
            ?? throw new InvalidOperationException("EmailSettings:Username not configured");
        
        _password = configuration["EmailSettings:Password"]
            ?? throw new InvalidOperationException("EmailSettings:Password not configured");
        
        _host = configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
        _port = int.Parse(configuration["EmailSettings:Port"] ?? "587");
        
        // Ne PAS connecter ici!
    }

    public void SendAccountCreationEmail(string to, string token)
    {
        //  CRÉER UNE NOUVELLE CONNEXION À CHAQUE ENVOI
        using var client = new SmtpClient();
        
        try
        {
            Console.WriteLine($" Connexion à {_host}:{_port}...");
            client.Connect(_host, _port, MailKit.Security.SecureSocketOptions.StartTls);
            Console.WriteLine($" Connecté!");
            
            Console.WriteLine($" Authentification avec {from}...");
            client.Authenticate(from, _password);
            Console.WriteLine($" Authentifié!");
            
            string link = $"{ORIGIN}/auth/signup?token={token}";
            Console.WriteLine($" Lien: {link}");
            
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Employee Service", from));
            message.To.Add(new MailboxAddress("", to));
            message.Subject = "Activate your account";
            message.Body = new TextPart("plain")
            {
                Text = $"Please click the following link to activate your account: {link}"
            };

            Console.WriteLine($" Envoi du message...");
            client.Send(message);
            Console.WriteLine($" Email envoyé à {to}!");
            
            client.Disconnect(true);
        }
        catch (Exception ex)
        {
            Console.WriteLine($" ERREUR SMTP: {ex.Message}");
            throw; // Pour voir l'erreur dans EmployeeServiceImp
        }
    }
}