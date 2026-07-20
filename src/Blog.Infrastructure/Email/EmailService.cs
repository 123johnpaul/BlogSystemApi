using System.Net;
using System.Net.Mail;
using Blog.Application.Interfaces.Email;
using Microsoft.Extensions.Configuration;

namespace Blog.Infrastructure.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body)
    {
        var smtp = _configuration.GetSection("Smtp");

        using var client = new SmtpClient(
            smtp["Host"],
            int.Parse(smtp["Port"]!));

        client.EnableSsl = bool.Parse(smtp["EnableSsl"]!);

        client.Credentials = new NetworkCredential(
            smtp["Username"],
            smtp["Password"]);

        using var message = new MailMessage
        {
            From = new MailAddress(
                smtp["FromEmail"]!,
                smtp["FromName"]),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(to);

        await client.SendMailAsync(message);
    }
}