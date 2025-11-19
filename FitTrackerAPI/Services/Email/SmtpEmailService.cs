using System.Net;
using System.Net.Mail;

namespace FitTrackerAPI.Services.Email;

public class SmtpEmailService: IEmailService
{
    private readonly IConfiguration _configuration;

    public SmtpEmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public Task SendEmailAsync(string to, string subject, string body)
    {
        var emailSettings = _configuration.GetSection("EmailSettings");

        using (var client = new SmtpClient(emailSettings["SmtpServer"], int.Parse(emailSettings["Port"]!)))
        {
            client.EnableSsl = bool.Parse(emailSettings["EnableSsl"]!);
            client.Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(emailSettings["SenderEmail"]!),
                Subject = subject,
                Body = body,
                IsBodyHtml = false // El código es texto simple
            };
            mailMessage.To.Add(to);

            try
            {
                client.Send(mailMessage);
            }
            catch (Exception ex)
            {
                // Manejo de errores: loggear el error
                Console.WriteLine($"Error al enviar email a {to}: {ex.Message}");
                // Puedes optar por relanzar el error o registrarlo en un log
            }
        }
        return Task.CompletedTask;
    }
}