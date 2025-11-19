namespace FitTrackerAPI.Services.Email;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        // En una app real, aquí se usaría SendGrid, Mailgun o SmtpClient.
        Console.WriteLine($"[EMAIL MOCK] Enviando a: {to}. Asunto: {subject}. Cuerpo: {body}");
        return Task.CompletedTask;
    }
}