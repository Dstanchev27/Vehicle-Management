namespace VMAPP.Services.Interfaces
{
    public interface ICustomEmailSender
    {
        Task SendEmailAsync(
            string from,
            string fromName,
            string to,
            string subject,
            string htmlContent);
    }
}
