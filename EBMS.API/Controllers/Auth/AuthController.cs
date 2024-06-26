using EBMS.Infrastructure.DTOs.Auth;
using EBMS.Infrastructure.Helpers;
using EBMS.Infrastructure.IServices.IAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EBMS.API.Controllers.Auth
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IBookAuthService _authService;

        public AuthController(IBookAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync(RegisterDTO model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterAsync(model);
            if(!result.IsAuthenticated)
                return BadRequest(result);

            // Setting The RefreshToken into Cookies
            SetRefreshTokenIntoCookies(result.RefreshToken!, result.RefreshTokenExpiration);

            string userUrl = Url.Link("getUser", new { username = result.UserName });
            return Created(userUrl, result);
        }

        [HttpPost("getToken")]
        public async Task<IActionResult> GetTokenAsync(GetTokenDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.GetTokenAsync(model);
            if (!result.IsAuthenticated)
                return BadRequest(result);

            // Setting The RefreshToken into Cookies
            SetRefreshTokenIntoCookies(result.RefreshToken!, result.RefreshTokenExpiration);

            return Ok(result);
        }

        [HttpGet("refreshToken")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            // Get The Refresh Token From Cookies
            var refreshToken = Request.Cookies["RefreshToken"];
            var newRT = await _authService.RefreshTokenAsync(refreshToken!);

            if(!newRT.IsAuthenticated)
                return BadRequest(newRT);

            // Store The New RT into Cookies
            SetRefreshTokenIntoCookies(newRT.RefreshToken!, newRT.RefreshTokenExpiration);

            return Ok(newRT);
        }

        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeTokenAsync(RevokeTokenDTO model)
        {
            var token = model.Token ?? Request.Cookies["RefreshToken"];
            if(string.IsNullOrEmpty(token))
                return BadRequest("RefreshToken is Required!");

            var result = await _authService.RevokeRefreshTokenAsync(token);
            if(!result) 
                return BadRequest("Invalid RefreshToken!");

            return Ok("RefreshToken Revoked Successfully!");
        }

        [HttpGet("user/{username}", Name = "getUser")]
        public async Task<IActionResult> GetUserAsync(string username)
        {
            if(string.IsNullOrEmpty(username))
                return BadRequest();

            var result = await _authService.GetUserAsync(username);
            if(!string.IsNullOrEmpty(result.Message))
                return NotFound(result.Message);

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin},{RolesConstants.Admin}")]
        [HttpGet("allUsers")]
        public async Task<IActionResult> GetAllUsersAsync()
        {
            var users = await _authService.GetAllUsersAsync();
            if (users is null)
                return BadRequest("No Users Yet!");

            return Ok(users);
        }

        [Authorize(Roles = $"{RolesConstants.Reader},{RolesConstants.Admin}")]
        [HttpPut("update/{username}", Name = "updateUser")]
        public async Task<IActionResult> UpdateUserAsync(string username, UpdateUserDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.UpdateUserAsync(username, model);
            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest($"{result.Message}");

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.Reader},{RolesConstants.Admin}")]
        [HttpPut("updatePwd/{username}", Name = "updateUserPwd")]
        public async Task<IActionResult> UpdateUserPwdAsync(string username, UpdatePwdDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.UpdateUserPwdAsync(username, model);
            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest($"{result.Message}");

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin}")]
        [HttpPost("addToRole")]
        public async Task<IActionResult> AddToRoleAsync(RoleDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AddToRoleAsync(model);
            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest($"{result.Message}");

            return Ok(result);
        }

        [Authorize(Roles = $"{RolesConstants.SuperAdmin}")]
        [HttpDelete("removeFromRole")]
        public async Task<IActionResult> RemoveFromRoleAsync(RoleDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RemoveFromRoleAsync(model);
            if (!string.IsNullOrEmpty(result.Message))
                return BadRequest($"{result.Message}");

            return Ok(result);
        }



        private void SetRefreshTokenIntoCookies(string refreshToken, DateTime expires)
        {
            var cookieOptions = new CookieOptions()
            {
                HttpOnly = true,
                Expires = expires.ToLocalTime()
            };

            Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);
        }
    }
}
