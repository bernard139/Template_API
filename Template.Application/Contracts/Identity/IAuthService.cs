using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Models.Identity;
using Template.Application.Responses;

namespace Template.Application.Contracts.Identity
{
    public interface IAuthService
    {
        Task<AuthResponse> Login(AuthRequest authRequest);
        Task<RegistrationResponse> Register(RegistrationRequest registrationRequest);
        Task<bool> VerifyEmailAsync(string email, string token);
        Task<bool> ForgotPasswordAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
