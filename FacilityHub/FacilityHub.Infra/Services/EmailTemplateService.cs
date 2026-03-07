using System.Reflection;
using FacilityHub.Core.Contracts;

namespace FacilityHub.Infra.Services;

public class EmailTemplateService(IEmailSender emailSender) : IEmailTemplateService
{
    private static readonly Assembly Assembly = typeof(EmailTemplateService).Assembly;

    private static string LoadTemplate(string templateName)
    {
        var resourceName = $"FacilityHub.Infra.EmailTemplates.{templateName}";
        using var stream = Assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException($"Email template '{templateName}' not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static string Render(string template, Dictionary<string, string> values)
    {
        foreach (var (key, value) in values)
            template = template.Replace($"{{{{{key}}}}}", value);
        return template;
    }

    public async Task SendEmailVerificationAsync(string to, string name, string verificationLink, CancellationToken cancellationToken)
    {
        var body = Render(LoadTemplate("EmailVerification.html"), new Dictionary<string, string>
        {
            ["Name"] = name,
            ["VerificationLink"] = verificationLink,
            ["Year"] = DateTime.UtcNow.Year.ToString()
        });
        await emailSender.SendEmailAsync(to, "Verify your FacilityHub email address", body, cancellationToken);
    }

    public async Task SendForgotPasswordAsync(string to, string name, string resetLink, string expiryMinutes, CancellationToken cancellationToken)
    {
        var body = Render(LoadTemplate("ForgotPassword.html"), new Dictionary<string, string>
        {
            ["Name"] = name,
            ["ResetLink"] = resetLink,
            ["ExpiryMinutes"] = expiryMinutes,
            ["Year"] = DateTime.UtcNow.Year.ToString()
        });
        await emailSender.SendEmailAsync(to, "Reset your FacilityHub password", body, cancellationToken);
    }

    public async Task SendLoginNotificationAsync(string to, string name, string ipAddress, string device, DateTime loginTime, CancellationToken cancellationToken)
    {
        var body = Render(LoadTemplate("LoginNotification.html"), new Dictionary<string, string>
        {
            ["Name"] = name,
            ["IpAddress"] = ipAddress,
            ["Device"] = device,
            ["LoginTime"] = loginTime.ToString("ddd, dd MMM yyyy HH:mm:ss") + " UTC",
            ["Year"] = DateTime.UtcNow.Year.ToString()
        });
        await emailSender.SendEmailAsync(to, "New login to your FacilityHub account", body, cancellationToken);
    }

    public async Task SendPasswordChangedAsync(string to, string name, CancellationToken cancellationToken)
    {
        var body = Render(LoadTemplate("PasswordChanged.html"), new Dictionary<string, string>
        {
            ["Name"] = name,
            ["ChangedAt"] = DateTime.UtcNow.ToString("ddd, dd MMM yyyy HH:mm:ss") + " UTC",
            ["Year"] = DateTime.UtcNow.Year.ToString()
        });
        await emailSender.SendEmailAsync(to, "Your FacilityHub password has been changed", body, cancellationToken);
    }
}
