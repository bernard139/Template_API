using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Models.Identity;

namespace Template.Application.Contracts.Identity
{
    public interface IUserService
    {
        Task<List<User>> GetUsers();
    }
}
