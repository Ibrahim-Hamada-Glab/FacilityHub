    using System.Security.Authentication;
using FacilityHub.Core.Contracts;
using FacilityHub.Core.helper;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace FacilityHub.Infra.Services;

public class EmailSender(IOptions<EmailConfig> emailConfigOptions, ILogger<EmailSender> logger)
    : IEmailSender
{
    private readonly EmailConfig _emailConfig = emailConfigOptions.Value;

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailConfig.DisplayName, _emailConfig.From));
            message.To.Add(new MailboxAddress(to, to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            using var client = new SmtpClient();
          
            client.Connect(_emailConfig.Host, _emailConfig.Port, false);
            client.Authenticate(_emailConfig.Username, _emailConfig.Password);
            client.Send(message);
            client.Disconnect(true);
            Console.WriteLine("Email sent successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to send email to {to}: {ex.Message}");
            throw new Exception($"Failed to send email to {to}", ex);
        }
    }
}