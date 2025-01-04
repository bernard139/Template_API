using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Responses;

namespace Template.Application.Contracts.Identity
{
    public interface IAccountActivationService
    {
        Task<ServerResponse<bool>> AccountActivation(string userId);
    }
}
