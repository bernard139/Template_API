using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Identity;
using Template.Application.Models.Identity;
using Template.Identity.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Template.Application.Contracts.Infrastructure;
using Template.Application.DTOs;
using Template.Application.Responses;
using Microsoft.AspNetCore.Http;
using Template.Application.Contracts.Persistence;
using Mapster;
using Microsoft.Extensions.Configuration;

namespace Template.Identity.Services
{
    public class AuthService : ResponseBaseService, IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailRequests _emailRequests;
        private readonly IEmailSender _emailSender;
        private readonly JwtSettings _jwtSettings;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IOptions<JwtSettings> jwtSettings, IEmailRequests emailRequests, IEmailSender emailSender, IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailRequests = emailRequests;
            _emailSender = emailSender;
            _jwtSettings = jwtSettings.Value;
            _configuration = configuration;
        }

        public async Task<ServerResponse<AuthResponse>> Login(AuthRequest authRequest)
        {
            var response = new ServerResponse<AuthResponse>();

            ApplicationUser applicationUser =  await _userManager.FindByEmailAsync(authRequest.Email);
            if (applicationUser == null)
            {
                return SetError(response, responseDescs.User_NOT_FOUND);
            }

            if (!applicationUser.EmailConfirmed)
            {
                return SetError(response, responseDescs.EMAIL_NOT_CONFIRMED);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(applicationUser, authRequest.Password);
            if (!isPasswordValid)
            {
                return SetError(response, responseDescs.INVALID_PASSWORD);
            }

            JwtSecurityToken jwtSecurityToken = await GenerateToken(applicationUser);

            response.Data = new AuthResponse
            {
                Id = applicationUser.Id,
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Email = applicationUser.Email,
                UserName = applicationUser.UserName
            };

            return SetSuccess(response, response.Data, responseDescs.SUCCESS);
        }

        public async Task<ServerResponse<RegistrationResponse>> Register(RegistrationRequest registrationRequest)
        {
            var response = new ServerResponse<RegistrationResponse>();

            var existingUser = await _userManager.FindByEmailAsync(registrationRequest.Email);
            if (existingUser == null)
            {
                var existingUsername = await _userManager.FindByNameAsync(registrationRequest.UserName);
                if (existingUsername != null)
                {
                    return SetError(response, responseDescs.USERNAME_EXISTS);
                }

                ApplicationUser user = registrationRequest.Adapt<ApplicationUser>();
                user.DateCreated = DateTime.Now;
                user.IsActive = true;
                user.IsDeleted = false;

                var result = await _userManager.CreateAsync(user, registrationRequest.Password);
                if (result.Succeeded)
                { 
                    await _userManager.AddToRoleAsync(user, "User");

                    var baseUrl = _configuration["SystemSettings:ApiBaseUrl"];
                    var accountActivationDto = new AccountActivationDTO
                    {
                        UserName = user.UserName,
                        Email = user.Email,
                        ActivationLink = $"{baseUrl}/Account/AccountActivate?userId={user.Id}"
                    };

                    var emailBody = await _emailRequests.AccountActivation(accountActivationDto);
                    var emailResponse = await _emailSender.SendEmail(emailBody);
                    if (emailResponse != null)
                    {
                        response.Data = new RegistrationResponse { UserId = user.Id };

                        return SetSuccess(response, response.Data, responseDescs.SUCCESS);
                    }
                    else
                    {
                        return SetError(response, responseDescs.EMAIL_NOT_SENT);
                    }
                }
                else
                {
                    return SetError(response, responseDescs.REGITRATION_FAILED);
                }
            }
            else
            {
                return SetError(response, responseDescs.USER_ALREADY_EXISTS);
            }
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
