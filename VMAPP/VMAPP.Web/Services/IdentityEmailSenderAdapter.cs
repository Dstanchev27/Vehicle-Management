using VMAPP.Services.Interfaces;

namespace VMAPP.Web.Services
{
    using Microsoft.AspNetCore.Identity.UI.Services;

    using System.Threading.Tasks;

    public class IdentityEmailSenderAdapter : IEmailSender
    {
        private readonly ICustomEmailSender emailSender;
        private readonly string senderEmail;
        private readonly string senderName;

        public IdentityEmailSenderAdapter(ICustomEmailSender emailSender, string senderEmail, string senderName)
        {
            this.emailSender = emailSender;
            this.senderEmail = senderEmail;
            this.senderName = senderName;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
            => this.emailSender.SendEmailAsync(this.senderEmail, this.senderName, email, subject, htmlMessage);
    }
}
