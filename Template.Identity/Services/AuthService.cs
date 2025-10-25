using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Identity;
using Template.Application.Contracts.Infrastructure;
using Template.Application.Contracts.Persistence;
using Template.Application.DTOs;
using Template.Application.DTOs.Identity;
using Template.Application.Models.Identity;
using Template.Application.Responses;
using Template.Identity.Models;

namespace Template.Identity.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;
        private readonly IEmailRequest _emailRequest;
        private readonly IUserService _userService;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings, IEmailRequest emailRequest, IUserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings.Value;
            _emailRequest = emailRequest;
            _userService = userService;
        }

        public async Task<AuthResponse> Login(AuthRequest authRequest)
        {
            ApplicationUser applicationUser = await _userManager.FindByEmailAsync(authRequest.Email);
            if (applicationUser == null)
            {
                throw new Exception($"ApplicationUser with {authRequest.Email} not found");
            }

            SignInResult result = await _signInManager.PasswordSignInAsync(applicationUser.UserName, authRequest.Password, false, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new Exception($"Credentials for {authRequest.Email} are not valid");
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(applicationUser);

            return new AuthResponse
            {
                Id = applicationUser.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = applicationUser.Email,
                UserName = applicationUser.UserName
            };
        }

        public async Task<RegistrationResponse> Register(RegistrationRequest registrationRequest)
        {
            var existingEmail = await _userManager.FindByEmailAsync(registrationRequest.Email);
            if (existingEmail == null)
            {
                ApplicationUser user = registrationRequest.Adapt<ApplicationUser>();
                user.UserName = registrationRequest.Email;
                user.DateCreated = DateTime.Now;
                var result = await _userManager.CreateAsync(user, registrationRequest.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    var userDto = user.Adapt<UserDto>();

                    var emailResult = await _emailRequest.SendVerificationEmail(userDto, token);
                    if (!emailResult)
                    {
                        throw new Exception($"Fsiled to send verification email to {registrationRequest.Email}.");
                    }
                    return new RegistrationResponse() { UserId = user.Id };
                }
                else
                {
                    throw new Exception($"{result.Errors}");
                }
            }
            else
            {
                throw new Exception($"Email {registrationRequest.Email} already exists.");
            }
        }

        public async Task<bool> VerifyEmailAsync(string email, string token)
        {
            var decodedToken = WebUtility.UrlDecode(token);

            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new Exception($"ApplicationUser with {email} not found");
            }

            user.IsActive = true;
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Email verification failed: {errors}");
            }
            return true;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            UserDto user = await _userService.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception($"ApplicationUser with {email} not found");
            }

            var token = await _userService.GeneratePasswordResetTokenAsync(email);

            var result = await _emailRequest.SendPasswordResetTokenEmail(user, token);
            if (!result)
            {
                throw new Exception($"Failed to send password reset token email to {email}.");
            }
            return true;
        }

        public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var decodedToken = WebUtility.UrlDecode(token);

            var result = await _userService.ResetPasswordAsync(email, decodedToken, newPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new Exception($"Failed to reset password: {errors}");
            }
            return true;
        }

        public async Task<JwtSecurityToken> GenerateToken(ApplicationUser applicationUser)
        {
            var userClaims = await _userManager.GetClaimsAsync(applicationUser);
            var roles = await _userManager.GetRolesAsync(applicationUser);

            var roleClaims = new List<Claim>();

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypes.Role, role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email),
                new Claim(JwtRegisteredClaimNames.Sub, applicationUser.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("firstName", applicationUser.FirstName),
                new Claim("uid", applicationUser.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }
    }
}
