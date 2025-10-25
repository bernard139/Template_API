using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Contracts.Identity;
using Template.Application.Models.Identity;

namespace Template.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login(AuthRequest request)
        {
            return Ok(await _authService.Login(request));
        }

        [HttpPost("register")]
        public async Task<ActionResult<RegistrationResponse>> Register(RegistrationRequest request)
        {
            return Ok(await _authService.Register(request));
        }
        [HttpPost("verify-email")]
        public async Task<ActionResult<RegistrationResponse>> VerifyEmail(string email, string token)
        {
            return Ok(await _authService.VerifyEmailAsync(email, token));
        }
        [HttpPost("forgot-password")]
        public async Task<ActionResult<RegistrationResponse>> ForgotPassword(string email)
        {
            return Ok(await _authService.ForgotPasswordAsync(email));
        }
        [HttpPost("reset-password")]
        public async Task<ActionResult<RegistrationResponse>> ResetPassword(string email, string token, string newPassword)
        {
            return Ok(await _authService.ResetPasswordAsync(email, token, newPassword));
        }
    }
}
