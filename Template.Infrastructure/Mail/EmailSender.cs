using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;
using Template.Application.Contracts.Infrastructure;
using Template.Application.Models;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace Template.Infrastructure.Mail
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailsettings;

        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailsettings = emailSettings.Value;
        }

        public async Task<bool> SendEmail(Email email)
        {
            // Try SendGrid first
            //var sendGridSuccess = await SendWithSendGrid(email);
            //if (sendGridSuccess) return true;

            // If SendGrid fails, fall back to SMTP
            return await SendWithSmtp(email);
        }

        private async Task<bool> SendWithSendGrid(Email email)
        {
            try
            {
                var client = new SendGridClient(_emailsettings.ApiKey);
                var to = new EmailAddress(email.To);
                var from = new EmailAddress(_emailsettings.FromAddress, _emailsettings.FromName);

                var message = MailHelper.CreateSingleEmail(from, to, email.Subject,
                                                          email.Body, email.Body);

                var response = await client.SendEmailAsync(message);
                return response.StatusCode == HttpStatusCode.OK ||
                       response.StatusCode == HttpStatusCode.Accepted;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> SendWithSmtp(Email email)
        {
            try
            {
                using (var smtpClient = new SmtpClient(_emailsettings.SmtpHost))
                {
                    smtpClient.Port = _emailsettings.SmtpPort;
                    smtpClient.Credentials = new NetworkCredential(
                        _emailsettings.SmtpUsername,
                        _emailsettings.SmtpPassword
                    );
                    smtpClient.EnableSsl = true;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(_emailsettings.FromAddress, _emailsettings.FromName);
                        mailMessage.To.Add(email.To);
                        mailMessage.Subject = email.Subject;
                        mailMessage.Body = email.Body;
                        mailMessage.IsBodyHtml = true;

                        await smtpClient.SendMailAsync(mailMessage);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
