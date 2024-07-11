using EBMS.Infrastructure.DTOs.Auth;
using EBMS.Infrastructure.Helpers;
using EBMS.Infrastructure.Helpers.Constants;
using EBMS.Infrastructure.IServices.IAuth;
using EBMS.Infrastructure.IServices.IFile;
using EBMS.Infrastructure.Models;
using EBMS.Infrastructure.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EBMS.Data.Services.Auth
{
    public class BookAuthService : IBookAuthService
    {
        private readonly UserManager<BookUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JWT _jwt;
        private readonly IFileService _fileService;

        public BookAuthService(UserManager<BookUser> userManager, RoleManager<IdentityRole> roleManager, IOptions<JWT> jwt, IFileService fileService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwt = jwt.Value;
            _fileService = fileService;
        }

        public async Task<BookAuthModel> RegisterAsync(RegisterDTO model)
        {
            var authModel = new BookAuthModel();
            // Check The Existence of Email
            if (await _userManager.FindByEmailAsync(model.Email) is not null)
            {
                authModel.Message = "Email already exists!!";
                return authModel;
            }
            // Check The Existence of UserName
            if (await _userManager.FindByNameAsync(model.UserName) is not null)
            {
                authModel.Message = "Username already exists!!";
                return authModel;
            }
            var newUser = new BookUser()
            {
                FullName = model.FullName,
                UserName = model.UserName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Created_at = DateTime.Now
            };

            // Check if there is a profile picture
            if (model.ProfilePic is not null)
            {
                var uploadImage = await _fileService.UploadFileAsBytesAsync(model.ProfilePic);

                newUser.ProfilePic = uploadImage.MemoryStream!.ToArray();
            }

            // Add Refresh Token for the User
            var refreshToken = GenerateRefreshToken();
            newUser.BookRefreshTokens?.Add(refreshToken);

            // Now, We can Create new user
            var result = await _userManager.CreateAsync(newUser, model.Password);
            // If there are some errors
            if (!result.Succeeded)
            {
                // Store The Errors Found
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},\n";
                }
                authModel.Message = errors;
                return authModel;
            }

            // One User will be SuperAdmin
            //result = await _userManager.AddToRoleAsync(newUser, RolesConstants.SuperAdmin);

            // By Default every User in the system is a Reader
            result = await _userManager.AddToRoleAsync(newUser, RolesConstants.Reader);
            if (!result.Succeeded)
            {
                // Store The Errors Found
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},\n";
                }
                authModel.Message = errors;
                return authModel;
            }


            // Generate new Token and log the user in
            var jwtToken = await GenerateJwtToken(newUser);
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.UserName = newUser.UserName;
            authModel.Email = newUser.Email;
            authModel.Roles = new List<string>() { RolesConstants.Reader };
            authModel.IsAuthenticated = true;
            authModel.Expires_at = jwtToken.ValidTo;
            authModel.RefreshToken = refreshToken.Token;
            authModel.RefreshTokenExpiration = refreshToken.Expires_at;

            return authModel;
        }

        public async Task<BookAuthModel> GetTokenAsync(GetTokenDTO model)
        {
            var authModel = new BookAuthModel();
            var user = await _userManager.FindByEmailAsync(model.Email);
            // Check if the user exists by Email
            if (user is null || !await _userManager.CheckPasswordAsync(user, model.Password))
            {
                authModel.Message = "Invalid Email or Password!!";
                return authModel;
            }
            // Get Current User Roles
            var roles = await _userManager.GetRolesAsync(user);
            // Generate New Token
            var jwtToken = await GenerateJwtToken(user);
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.UserName = user.UserName!;
            authModel.Email = user.Email!;
            authModel.Roles = roles.ToList();
            authModel.IsAuthenticated = true;
            authModel.Expires_at = jwtToken.ValidTo;

            // Check if the user has RefreshToken
            if (user.BookRefreshTokens is not null && user.BookRefreshTokens.Any(x => x.IsActive))
            {
                var activeRT = user.BookRefreshTokens.First(x => x.IsActive);
                authModel.RefreshToken = activeRT.Token;
                authModel.RefreshTokenExpiration = activeRT.Expires_at;
            }
            else
            {
                // Generate New RefreshToken
                var refreshToken = GenerateRefreshToken();
                authModel.RefreshToken = refreshToken.Token;
                authModel.RefreshTokenExpiration = refreshToken.Expires_at;
                user.BookRefreshTokens?.Add(refreshToken);
                // Now, we should update user in database
                user.Updated_at = DateTime.Now;
                await _userManager.UpdateAsync(user);
            }

            return authModel;
        }

        public async Task<BookAuthModel> RefreshTokenAsync(string refreshToken)
        {
            var authModel = new BookAuthModel();
            // Be careful! Here Any() ignores case when comparing two strings
            var user = _userManager.Users.SingleOrDefault(x => x.BookRefreshTokens!.Any(y => y.Token == refreshToken));
            if(user is null)
            {
                authModel.Message = "Invalid Refresh Token!";
                return authModel;
            }
            var token = user.BookRefreshTokens!.SingleOrDefault(x => x.Token == refreshToken);
            if(token is null)
            {
                authModel.Message = "Invalid Refresh Token!";
                return authModel;
            }
            if (!token.IsActive)
            {
                authModel.Message = "InActive Refresh Token!";
                return authModel;
            }
            // mark The current RT as Revoked
            token.Revoked_at = DateTime.Now;
            // Generate New Refresh Token
            var newRT = GenerateRefreshToken(); 
            // Add new RT to database
            user.BookRefreshTokens.Add(newRT);
            await _userManager.UpdateAsync(user);
            // Generate New JWT Token
            var jwtToken = await GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);
            authModel.Token = new JwtSecurityTokenHandler().WriteToken(jwtToken);
            authModel.UserName = user.UserName!;
            authModel.Email = user.Email!;
            authModel.Roles = roles.ToList();
            authModel.IsAuthenticated = true;
            authModel.Expires_at = jwtToken.ValidTo;
            authModel.RefreshToken = newRT.Token;
            authModel.RefreshTokenExpiration = newRT.Expires_at;

            return authModel;
        }

        public async Task<bool> RevokeRefreshTokenAsync(string refreshToken)
        {
            // Be careful! Here Any() ignores case when comparing two strings
            var user = _userManager.Users.SingleOrDefault(x => x.BookRefreshTokens.Any(x => x.Token == refreshToken));
            if (user is null)
                return false;

            var token = user.BookRefreshTokens!.SingleOrDefault(x => x.Token == refreshToken);
            if (token is null || !token.IsActive)
                return false;

            // Mark this token as revoked (becomes inactive)
            token.Revoked_at = DateTime.Now;
            // Update User RefreshToken
            await _userManager.UpdateAsync(user);

            return true;
        }

        public async Task<GetUserDTO> GetUserAsync(string username)
        {
            var userDTO = new GetUserDTO();
            // Check If There is a user with this username
            var user = await _userManager.FindByNameAsync(username);
            if (user is null || username == SuperAdminConstants.UserName)
            {
                userDTO.Message = "User is not Found!";
                return userDTO;
            }

            // Update User DTO
            userDTO = await UserDTO(user);

            return userDTO;
        }

        public async Task<List<GetUserDTO>> GetAllUsersAsync()
        {
            var allUsers = new List<GetUserDTO>();
            var users = _userManager.Users.ToList();
            if (users is null)
                return null!;

            foreach (var user in users)
            {
                if (user.UserName == SuperAdminConstants.UserName)
                    continue;
                allUsers.Add(await UserDTO(user));
            }
            return allUsers;
        }

        public async Task<GetUserDTO> UpdateUserAsync(string username, UpdateUserDTO model)
        {
            var userDTO = new GetUserDTO();
            // Get The actual user from database
            var updatedUser = await _userManager.FindByNameAsync(username);
            if (updatedUser is null)
            {
                userDTO.Message = "This User is not Found!";
                return userDTO;
            }
            // Check if the username had taken before
            if (username.ToLowerInvariant() != model.UserName.ToLowerInvariant())
            {
                var user = await _userManager.FindByNameAsync(model.UserName);
                if(user is not null)
                {
                    userDTO.Message = "Username Already Exists!";
                    return userDTO;
                }

            }
            // Update User Data
            updatedUser.FullName = model.FullName;
            updatedUser.UserName = model.UserName;
            updatedUser.PhoneNumber = model.PhoneNumber;
            updatedUser.DateOfBirth = model.DateOfBirth;
            // Check if there is a profile picture
            if (model.ProfilePic is not null)
            {
                var uploadImage = await _fileService.UploadFileAsBytesAsync(model.ProfilePic);

                updatedUser.ProfilePic = uploadImage.MemoryStream!.ToArray();
            }

            // Update Data in DB
            updatedUser.Updated_at = DateTime.Now;
            var result = await _userManager.UpdateAsync(updatedUser);
            // If there are some errors
            if (!result.Succeeded)
            {
                // Store The Errors Found
                var errors = string.Empty;
                foreach (var error in result.Errors)
                {
                    errors += $"{error.Description},\n";
                }
                userDTO.Message = errors;
                return userDTO;
            }

            // Update User DTO
            userDTO = await UserDTO(updatedUser);

            return userDTO;
        }

        public async Task<GetUserDTO> UpdateUserPwdAsync(string username, UpdatePwdDTO model)
        {
            var userDTO = new GetUserDTO();
            // Get The actual user from database
            var updatedUser = await _userManager.FindByNameAsync(username);
            if (updatedUser is null)
            {
                userDTO.Message = "This User is not Found!";
                return userDTO;
            }
            // Check the password and change it
            var pwdResult = await _userManager.ChangePasswordAsync(updatedUser, model.OldPwd, model.NewPwd);
            // If there are some errors
            if (!pwdResult.Succeeded)
            {
                // Store The Errors Found
                var errors = string.Empty;
                foreach (var error in pwdResult.Errors)
                {
                    errors += $"{error.Description},\n";
                }
                userDTO.Message = errors;
                return userDTO;
            }
            // After Changing user password
            updatedUser.Updated_at = DateTime.Now;
            var updateResult = await _userManager.UpdateAsync(updatedUser);
            // If there are some errors
            if (!updateResult.Succeeded)
            {
                // Store The Errors Found
                var errors = string.Empty;
                foreach (var error in updateResult.Errors)
                {
                    errors += $"{error.Description},\n";
                }
                userDTO.Message = errors;
                return userDTO;
            }

            // Update User DTO
            userDTO = await UserDTO(updatedUser);

            return userDTO;
        }

        public async Task<GetUserDTO> AddToRoleAsync(RoleDTO model)
        {
            var userDTO = new GetUserDTO();
            // Get The actual user from database
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                userDTO.Message = "This User is not Found!";
                return userDTO;
            }
            // Check if the role exists
            if (model.Role.ToLowerInvariant() == RolesConstants.SuperAdmin || !await _roleManager.RoleExistsAsync(model.Role))
            {
                userDTO.Message = "Invalid Role!";
                return userDTO;
            }
            // Check if the user already assigned to this role
            if (await _userManager.IsInRoleAsync(user, model.Role))
            {
                userDTO.Message = "User is already in this Role!";
                return userDTO;
            }
            // Add The User To Role
            var addToRole = await _userManager.AddToRoleAsync(user, model.Role);
            // If there are some errors
            if (!addToRole.Succeeded)
            {
                // Store The Errors Found
                var errors = string.Empty;
                foreach (var error in addToRole.Errors)
                {
                    errors += $"{error.Description},\n";
                }
                userDTO.Message = errors;
                return userDTO;
            }

            // Update User DTO
            userDTO = await UserDTO(user);

            return userDTO;
        }

        public async Task<GetUserDTO> RemoveFromRoleAsync(RoleDTO model)
        {
            var userDTO = new GetUserDTO();
            // Get The actual user from database
            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user is null)
            {
                userDTO.Message = "This User is not Found!";
                return userDTO;
            }
            // Check if the role exists
            if (model.Role.ToLowerInvariant() == RolesConstants.SuperAdmin || !await _roleManager.RoleExistsAsync(model.Role))
            {
                userDTO.Message = "Invalid Role!";
                return userDTO;
            }
            // Check if the user in this role
            if (!await _userManager.IsInRoleAsync(user, model.Role))
            {
                userDTO.Message = "User is not in this Role!";
                return userDTO;
            }
            // Check if the user has already one role
            var roles = await _userManager.GetRolesAsync(user);
            if(roles.Count <= 1)
            {
                userDTO.Message = "User should be assigned to at least one role!";
                return userDTO;
            }

            // Remove The User From Role
            var removeFromRole = await _userManager.RemoveFromRoleAsync(user, model.Role);
            // If there are some errors
            if (!removeFromRole.Succeeded)
            {
                // Store The Errors Found
                var errors = string.Empty;
                foreach (var error in removeFromRole.Errors)
                {
                    errors += $"{error.Description},\n";
                }
                userDTO.Message = errors;
                return userDTO;
            }

            // Update User DTO
            userDTO = await UserDTO(user);

            return userDTO;
        }



        // Get UserDTO
        private async Task<GetUserDTO> UserDTO(BookUser user)
        {
            var userDTO = new GetUserDTO();
            var roles = await _userManager.GetRolesAsync(user);
            var refreshToken = user.BookRefreshTokens?.FirstOrDefault(t => t.IsActive);
            userDTO.FullName = user.FullName;
            userDTO.UserName = user.UserName;
            userDTO.Email = user.Email;
            userDTO.PhoneNumber = user.PhoneNumber;
            userDTO.DateOfBirth = user.DateOfBirth;
            userDTO.ProfilePic = user.ProfilePic;
            userDTO.Roles = roles.ToList();
            userDTO.RefreshTokenExpiration = refreshToken?.Expires_at;

            return userDTO;
        }

        // Token Generation Factory
        private async Task<JwtSecurityToken> GenerateJwtToken(BookUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var userRoles = await _userManager.GetRolesAsync(user);
            var rolesClaims = new List<Claim>();

            foreach (var role in userRoles)
            {
                rolesClaims.Add(new Claim("roles", role));
            }

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
            }
            .Union(userClaims)
            .Union(rolesClaims);

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(_jwt.TokenDurationInDays),
                signingCredentials: signingCredentials
            );

            return jwtSecurityToken;
        }
        private BookRefreshToken GenerateRefreshToken()
        {
            var randomNum = new byte[32];
            using var generator = new RNGCryptoServiceProvider();

            generator.GetBytes(randomNum);

            return new BookRefreshToken
            {
                Token = Convert.ToBase64String(randomNum),
                Expires_at = DateTime.Now.AddDays(_jwt.RefreshTokenDurationInDays),
                Created_at = DateTime.Now
            };
        }
    }
}
