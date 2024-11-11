using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using Template.Application.Contracts.Infrastructure;
using Template.Application.Models;

namespace Template.Infrastructure.Mail
{
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailsettings {  get; }
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailsettings = emailSettings.Value;
        }
        public async Task<bool> SendEmail(Email email)
        {
            SendGridClient client = new(_emailsettings.ApiKey);
            EmailAddress to = new(email.To);
            EmailAddress from = new()
            {
                Email = _emailsettings.FromAddress,
                Name = _emailsettings.FromName
            };

            SendGridMessage message = MailHelper.CreateSingleEmail(from, to, email.Subject,
                                                                   email.Body, "EmailContent");
            Response response = await client.SendEmailAsync(message);

            return response.StatusCode == System.Net.HttpStatusCode.OK ||
                   response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
    }
}
