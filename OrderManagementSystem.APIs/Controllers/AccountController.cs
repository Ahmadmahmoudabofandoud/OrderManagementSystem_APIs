using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Core;
using OrderManagementSystem.Core.Entities.UserAggregate;
using OrderManagementSystem.Core.Services.Contract;
using System.Security.Cryptography;
using System.Text;

namespace OrderManagementSystem.APIs.Controllers
{
    
    public class AccountController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;

        public AccountController(IUnitOfWork unitOfWork , ITokenService tokenService)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
        }

        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            if (await UserExists(registerDto.Username)) 
                return BadRequest(new ApiResponse(404 , "Username already exist"));
            var hmac = new HMACSHA512();

            var user = new User
            {
                UserName = registerDto.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password)),
                PasswordSalt = hmac.Key,
                Role = registerDto.Role
            };
            _unitOfWork.Repository<User>().Add(user);
            await _unitOfWork.CompleteAsync();

            return Ok(new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
        {
            var user = await _unitOfWork.Repository<User>()
                .SingleOrDefaultAsync(x => x.UserName == loginDto.Username);

            if (user == null) 
                return Unauthorized(new ApiResponse(401, "Invalid UserName"));

            var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized(new ApiResponse(401 , "Invalid Password"));
            }

            return Ok(new UserDto()
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            });
        }

        private async Task<bool> UserExists(string username)
            => await _unitOfWork.Repository<User>().GetAnyAsync(username);

    }
}
