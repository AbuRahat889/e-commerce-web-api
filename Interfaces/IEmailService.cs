
namespace e_commerce_web_api.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync(
        string toEmail,
        string subject,
        string body);
}
