using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Contracts.Identity;
using Template.Application.Models.Identity;
using Template.Identity.Services;

namespace Template.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("get-users")]
        public async Task<ActionResult<List<User>>> GetUsers()
        {
            return Ok(await _userService.GetUsers());
        }

        [HttpGet("get-user-by-email")]
        public async Task<ActionResult<List<User>>> GetUserByEmail(string email)
        {
            return Ok(await _userService.GetUserByEmail(email));
        }

        [HttpGet("get-userRoles")]
        public async Task<ActionResult<IList<string>>> GetUserRoles(string id)
        {
            return Ok(await _userService.GetUserRoles(id));
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<ChangePasswordResponse>> ChangePassword(ChangePasswordRequest request)
        {
            request.UserId = GetCurrentUserId();
            return Ok(await _userService.ChangePassword(request));
        }
    }
}
