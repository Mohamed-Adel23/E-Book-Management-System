using EBMS.Infrastructure.DTOs.Auth;
using EBMS.Infrastructure.Models.Auth;

namespace EBMS.Infrastructure.IServices.IAuth
{
    public interface IBookAuthService
    {
        Task<BookAuthModel> RegisterAsync(RegisterDTO model); // Register
        Task<BookAuthModel> GetTokenAsync(GetTokenDTO model); // Login
        Task<BookAuthModel> RefreshTokenAsync(string refreshToken); // RefreshToken
        Task<bool> RevokeRefreshTokenAsync(string refreshToken); // Revoke RefreshToken
        Task<GetUserDTO> GetUserAsync(string username); // Get User By UserName
        Task<List<GetUserDTO>> GetAllUsersAsync(); // Get All Users
        Task<GetUserDTO> UpdateUserAsync(string username, UpdateUserDTO model); // Update User Data
        Task<GetUserDTO> UpdateUserPwdAsync(string username, UpdatePwdDTO model); // Update User Password
        // These Actions For SuperAdmin Only
        Task<GetUserDTO> AddToRoleAsync(RoleDTO model); // Add User To New Role
        Task<GetUserDTO> RemoveFromRoleAsync(RoleDTO model); // Remove User From Role
    }
}
