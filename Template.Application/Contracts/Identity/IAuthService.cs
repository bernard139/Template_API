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
        Task<ServerResponse<AuthResponse>> Login(AuthRequest authRequest);
        Task<ServerResponse<RegistrationResponse>> Register(RegistrationRequest registrationRequest);
    }
}
