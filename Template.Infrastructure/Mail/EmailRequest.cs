using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Infrastructure;
using Template.Application.DTOs;
using Template.Application.DTOs.Identity;
using Template.Application.Models;
using Template.Identity.Models;

namespace Template.Infrastructure.Mail
{
    public class EmailRequest : IEmailRequest
    {
        private readonly IEmailSender _emailSender;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EmailRequest(IEmailSender emailSender, IHttpContextAccessor httpContextAccessor)
        {
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> SendVerificationEmail(UserDto user, string token)
        {
            var encodedToken = Uri.EscapeDataString(token);

            var baseUrl = GetBaseUrl();
            var verificationUrl = $"{baseUrl.TrimEnd('/')}/api/account/verify-email?email={user.Email}&token={encodedToken}";

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "EmailVerification.html");
            var templateContent = await File.ReadAllTextAsync(templatePath);

            var emailBody = templateContent
                .Replace("{{VerificationUrl}}", verificationUrl)
                .Replace("{{Email}}", user.Email)
                .Replace("{{Initials}}", GetUserInitials(user.FirstName, user.LastName))
                .Replace("{{FirstName}}", user.FirstName)
                .Replace("{{LastName}}", user.LastName ?? "")
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString())
                .Replace("{{SignupDate}}", user.DateCreated.ToString("dd MMM, yyyy"))
                .Replace("{{ExpirationHours}}", "24");

            var email = new Email
            {
                To = user.Email,
                Subject = "Verify your PayLink account",
                Body = emailBody
            };

            var result = await _emailSender.SendEmail(email);
            if (!result) { return false; }
            return true;
        }
        public async Task<bool> SendPasswordResetTokenEmail(UserDto user, string token)
        {
            var encodedToken = Uri.EscapeDataString(token);

            var baseUrl = GetBaseUrl();
            var resetUrl = $"{baseUrl.TrimEnd('/')}/api/account/reset-password?email={user.Email}&token={encodedToken}";

            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "PasswordResetEmail.html");
            var templateContent = await File.ReadAllTextAsync(templatePath);

            var emailBody = templateContent
                .Replace("{{ResetUrl}}", resetUrl)
                .Replace("{{FirstName}}", user.FirstName)
                .Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

            var email = new Email
            {
                To = user.Email,
                Subject = "Reset your PayLink password",
                Body = emailBody
            };

            var result = await _emailSender.SendEmail(email);
            if (!result) { return false; }
            return true;
        }

        // Helper method for initials
        private string GetUserInitials(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) && string.IsNullOrEmpty(lastName))
                return "PL";

            var firstInitial = !string.IsNullOrEmpty(firstName) ? firstName[0].ToString().ToUpper() : "";
            var lastInitial = !string.IsNullOrEmpty(lastName) ? lastName[0].ToString().ToUpper() : "";

            return $"{firstInitial}{lastInitial}";
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                return "https://paylink.com";
            }

            var scheme = request.Scheme;
            var host = request.Host.ToUriComponent();

            return $"{scheme}://{host}";
        }
    }
}
