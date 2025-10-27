using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Template.Application.Contracts.Identity;
using Template.Application.DTOs.Identity;
using Template.Application.Models.Enums;
using Template.Application.Models.Identity;
using Template.Identity.Services;

namespace Template.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<UserDto>>> GetUsers()
        {
            var users = await _userService.GetUsers();
            return Ok(users);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> UpdateUser(UpdateUserDto updateRequest)
        {
            string userId = GetCurrentUserId();
            var result = await _userService.UpdateUserAsync(updateRequest, userId);
            return Ok(result);
        }

        [HttpPost("upload-image")]
        public async Task<ActionResult<bool>> UploadImage([FromForm] ImageUploadDto request)
        {
            string userId = GetCurrentUserId();
            UploadType type = UploadType.User;
            var result = await _userService.UploadImage(request.File, userId, type);
            return Ok(result);
        }

        [HttpGet("user-roles")]
        public async Task<ActionResult<IList<string>>> GetUserRoles()
        {
            string id = GetCurrentUserId();
            var roles = await _userService.GetUserRoles(id);
            return Ok(roles);
        }

        [HttpGet("get-by-email")]
        public async Task<ActionResult<UserDto>> GetUserByEmail([FromQuery] string email)
        {
            var user = await _userService.GetUserByEmail(email);
            return Ok(user);
        }

        [HttpPost("change-password")]
        public async Task<ActionResult<IdentityResult>> ChangePassword(string currentPassword, string newPassword)
        {
            string userId = GetCurrentUserId();
            var result = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);
            return Ok(result);
        }

        [HttpPost("activate")]
        public async Task<ActionResult<IdentityResult>> ActivateUser()
        {
            string userId = GetCurrentUserId();
            var result = await _userService.ActivateUserAsync(userId);
            return Ok(result);
        }

        [HttpPost("deactivate")]
        public async Task<ActionResult<IdentityResult>> DeactivateUser()
        {
            string userId = GetCurrentUserId();
            var result = await _userService.DeactivateUserAsync(userId);
            return Ok(result);
        }

        [HttpPost("delete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IdentityResult>> DeleteUser(string userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            return Ok(result);
        }
    }
}
