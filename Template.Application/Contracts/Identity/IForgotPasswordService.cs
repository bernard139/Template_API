using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Models.Identity;

namespace Template.Application.Contracts.Identity
{
    public interface IForgotPasswordService
    {
        Task<bool> SendForgotPasswordOTP(string email);
        Task<bool> ResetPassword(ResetPasswordRequest request);
    }
}
