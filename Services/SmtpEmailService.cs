
using System.Net;
using System.Net.Mail;
using e_commerce_web_api.Configurations;
using e_commerce_web_api.Interfaces;
using Microsoft.Extensions.Options;

namespace e_commerce_web_api.Services;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _emailSettings;

    public SmtpEmailService(IOptions<EmailSettings> emailSettings)
    {
        _emailSettings = emailSettings.Value;
    }

    public async Task SendEmailAsync(
        string toEmail,
        string subject,
        string body)
    {   
        using var message = new MailMessage();

        message.From = new MailAddress(
            _emailSettings.SenderEmail,
            _emailSettings.SenderName);

        message.To.Add(toEmail);
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = false;

        using var smtpClient = new SmtpClient(
            _emailSettings.SmtpServer,
            _emailSettings.SmtpPort);

        smtpClient.EnableSsl = true;
        smtpClient.Credentials = new NetworkCredential(
            _emailSettings.Username,
            _emailSettings.Password);

        await smtpClient.SendMailAsync(message);
    }
}
