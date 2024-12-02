using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.DTOs;
using Template.Application.Models;

namespace Template.Application.Contracts.Infrastructure
{
    public interface IEmailRequests
    {
        Task<EmailBody> PasswordOTPEmailRequest(UserDTO user, string otp);
    }
}
 