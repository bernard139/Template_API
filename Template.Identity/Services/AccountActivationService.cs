using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Application.Contracts.Identity;
using Template.Application.Contracts.Persistence;
using Template.Application.Responses;
using Template.Identity.Models;

namespace Template.Identity.Services
{
    public class AccountActivationService : ResponseBaseService, IAccountActivationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<AccountActivationService> _logger;

        public AccountActivationService(UserManager<ApplicationUser> userManager, ILogger<AccountActivationService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<ServerResponse<bool>> AccountActivation(string userId)
        {
            var response = new ServerResponse<bool>();

            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return SetError(response, responseDescs.INVALID_USER);
            }

            if (user.EmailConfirmed)
            {
                return SetError(response, responseDescs.FAIL);
            }

            user.EmailConfirmed = true;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return SetError(response, responseDescs.REQUEST_NOT_SUCCESSFUL);
            }

            return SetSuccess(response, true, responseDescs.SUCCESS);
        }
    }
}
