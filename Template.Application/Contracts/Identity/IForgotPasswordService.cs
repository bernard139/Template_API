using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Models.Identity;
using Template.Application.Responses;

namespace Template.Application.Contracts.Identity
{
    public interface IForgotPasswordService
    {
        Task<ServerResponse<bool>> SendForgotPasswordOTP(string email);
        Task<ServerResponse<bool>> ResetPassword(ResetPasswordRequest request);
    }
}
