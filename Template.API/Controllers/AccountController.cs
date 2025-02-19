﻿using Microsoft.AspNetCore.Authorization;
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
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly IAccountActivationService _accountActivationService;

        public AccountController(IAuthService authService, IForgotPasswordService forgotPasswordService, IAccountActivationService accountActivationService)
        {
            _authService = authService;
            _forgotPasswordService = forgotPasswordService;
            _accountActivationService = accountActivationService;
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

        [HttpPost("activate-account")]
        public async Task<ActionResult<bool>> ActivateAccount(string userId)
        {
            return Ok(await _accountActivationService.AccountActivation(userId));
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<bool>> SendForgotPasswordOTP(string email)
        {
            return Ok(await _forgotPasswordService.SendForgotPasswordOTP(email));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<bool>> ResetPassword(ResetPasswordRequest request)
        {
            return Ok(await _forgotPasswordService.ResetPassword(request));
        }
    }
}
