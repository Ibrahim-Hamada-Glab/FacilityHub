namespace FacilityHub.Core.Contracts;

public interface IEmailTemplateService
{
    Task SendEmailVerificationAsync(string to, string name, string verificationLink, CancellationToken cancellationToken);
    Task SendForgotPasswordAsync(string to, string name, string resetLink, string expiryMinutes, CancellationToken cancellationToken);
    Task SendLoginNotificationAsync(string to, string name, string ipAddress, string device, DateTime loginTime, CancellationToken cancellationToken);
    Task SendPasswordChangedAsync(string to, string name, CancellationToken cancellationToken);
}
