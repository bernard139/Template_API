
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Template.Identity.Models;
using Template.Application.Responses;
using Template.Application.Contracts.Identity;
using Template.Application.Contracts.Infrastructure;
using Template.Application.Misc;
using Microsoft.EntityFrameworkCore;
using Template.Identity.Models.Enums;
using Template.Application.Models;
using Template.Application.Models.Enums;
using Template.Application.Models.Identity;
using Template.Application.DTOs;

namespace Template.Identity.Services
{
    public class ForgotPasswordService : BaseCommandResponse, IForgotPasswordService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ForgotPasswordService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IEmailRequests _emailRequest;

        private readonly TemplateIdentityDbContext _context;
        private readonly IConfiguration _configuration;
        public ForgotPasswordService(UserManager<ApplicationUser> userManager, ILogger<ForgotPasswordService> logger,
                                     IEmailSender emailSender,
                                     TemplateIdentityDbContext context, IConfiguration configuration, IHttpContextAccessor contextAccessor, IEmailRequests emailRequest)
        {
            _userManager = userManager;
            _logger = logger;
            _configuration = configuration;
            _emailSender = emailSender;
            _context = context;
            _emailRequest = emailRequest;
        }

        public async Task<bool> SendForgotPasswordOTP(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception($"ApplicationUser with {email} not found");
            }

            string otp = Utilities.GenerateOTP();

            string otpHashed = Utilities.EncrytptionDecryptionHelper.Hash(otp);
            var data = await _context.OTPs.FirstOrDefaultAsync(p => p.Email == email && p.OTPType == OTPType.ResetPassword.ToString());
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            if (data is null)
            {
                var otpEntry = new OTPs
                {
                    OTP = otpHashed,
                    Email = email,
                    OTPType = OTPType.ResetPassword.ToString(),
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    Token = token
                };

                await _context.OTPs.AddAsync(otpEntry);

            }
            else
            {
                data.OTP = otpHashed;
                data.CreatedDate = DateTime.Now;
                data.IsActive = true;
                data.Token = token;
            }

            long save = await _context.SaveChangesAsync();
            if (save > 0)
            {
                UserDTO userDTO = new UserDTO
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = email,
                    UserName = user.UserName
                };

                EmailBody emailBody = await _emailRequest.PasswordOTPEmailRequest(userDTO, otp);
                var emailResponse = await _emailSender.SendEmail(emailBody);
                if (emailResponse != null)
                {
                    return true;
                }
                else
                {
                    _logger.LogInformation("Reset password  failed due to failed mail");
                    return false;
                }
            }
            else
            {
                _logger.LogInformation("Could not save or update the otp");
                return false;
            }
        }

        public async Task<bool> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                throw new Exception($"ApplicationUser with {request.Email} not found");
            }

            var hashedOtp = Utilities.EncrytptionDecryptionHelper.Hash(request.OTP);
            var otpEntry = await _context.OTPs.FirstOrDefaultAsync(p => p.Email == request.Email && p.OTPType == OTPType.ResetPassword.ToString());

            if (otpEntry == null)
            {
                
            }
            bool isVerified = Utilities.EncrytptionDecryptionHelper.Verify(request.OTP, otpEntry.OTP);

            if (!isVerified)
            {
                throw new Exception($"Invalid OTP");
            }

            var passwordValidator = new PasswordValidator<ApplicationUser>();

            PasswordVerificationResult passwordVerificationResult = _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, request.NewPassword);
            if (passwordVerificationResult == PasswordVerificationResult.Success || passwordVerificationResult == PasswordVerificationResult.SuccessRehashNeeded)
            {
                _logger.LogInformation("You can not reset with same password");
                throw new Exception($"Same password error");
            }
            var passwordValidationResult = await passwordValidator.ValidateAsync(_userManager, user, request.NewPassword);
            if (passwordValidationResult.Succeeded == false)
            {
                _logger.LogInformation("Password did not meet the required criteria");
                throw new Exception($"Invalid Password");
            }
            else
            {
                otpEntry.IsActive = false;
                _context.OTPs.Update(otpEntry);
                bool isUpdated = (await _context.SaveChangesAsync()) > 0;
                if (!isUpdated)
                {
                    _logger.LogInformation("Could not update the OTP status");
                    throw new Exception($"Failed to update OTP status");
                }
                else
                {
                    var resetPasswordResult = await _userManager.ResetPasswordAsync(user, otpEntry.Token, request.NewPassword);
                    if (resetPasswordResult.Succeeded)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception($"Failed to Reset Password");

                    }
                }
            }
        }
    }
}
