using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.DTOs;
using Template.Application.DTOs.Identity;
using Template.Application.Models;

namespace Template.Application.Contracts.Infrastructure
{
    public interface IEmailRequest
    {
        Task<bool> SendVerificationEmail(UserDto user, string token);
        Task<bool> SendPasswordResetTokenEmail(UserDto user, string token);
    }
}
 