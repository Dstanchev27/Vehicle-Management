namespace VMAPP.Services
{
    using SendGrid;
    using SendGrid.Helpers.Mail;

    using System;
    using System.Threading.Tasks;

    using VMAPP.Services.Interfaces;

    public class SendGridEmailSender : ICustomEmailSender
    {
        private readonly SendGridClient client;

        public SendGridEmailSender(string apiKey)
        {
            this.client = new SendGridClient(apiKey);
        }

        public async Task SendEmailAsync(string from, string fromName, string to, string subject, string htmlContent)
        {
            if (string.IsNullOrWhiteSpace(subject) && string.IsNullOrWhiteSpace(htmlContent))
            {
                throw new ArgumentException("Subject and message should be provided.");
            }

            var fromAddress = new EmailAddress(from, fromName);
            var toAddress = new EmailAddress(to);
            var message = MailHelper.CreateSingleEmail(fromAddress, toAddress, subject, null, htmlContent);

            await this.client.SendEmailAsync(message);
        }
    }
}
