using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Infrastructure;
using Template.Application.DTOs;
using Template.Application.Models;
using Template.Identity.Models;

namespace Template.Infrastructure.Mail
{
    public class EmailRequests : IEmailRequests
    {
        public EmailRequests()
        {
        }

        public async Task<EmailBody> AccountActivation(AccountActivationDTO request)
        {

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "AccountActivation.html");
            var messageBody = await System.IO.File.ReadAllTextAsync(templatePath);

            messageBody = messageBody.Replace("{UserName}", request.UserName);
            messageBody = messageBody.Replace("{ActivationLink}", request.ActivationLink);

            var emailBody = new EmailBody
            {
                MessageBody = messageBody,
                Receiver = request.Email,
                Subject = "Account Activation",
                HasAttachment = false,
            };
            return emailBody;
        }

        public async Task<EmailBody> PasswordOTPEmailRequest(UserDTO user, string otp)
        {

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ResetPasswordOTP.html");
            var messageBody = await System.IO.File.ReadAllTextAsync(templatePath);

            messageBody = messageBody.Replace("{UserName}", user.UserName);
            messageBody = messageBody.Replace("{OTP}", otp);

            var emailBody = new EmailBody
            {
                MessageBody = messageBody,
                Receiver = user.Email,
                Subject = "Password Reset OTP",
                HasAttachment = false,
            };
            return emailBody;
        }
    }
}
