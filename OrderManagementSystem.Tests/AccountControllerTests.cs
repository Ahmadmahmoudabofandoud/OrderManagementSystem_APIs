using Moq;
using OrderManagementSystem.APIs.Controllers;
using OrderManagementSystem.Core.Services.Contract;
using OrderManagementSystem.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.APIs.DTOs;
using OrderManagementSystem.APIs.Errors;
using OrderManagementSystem.Core.Entities.UserAggregate;
using System.Security.Cryptography;

namespace OrderManagementSystem.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AccountController _controller;
        public AccountControllerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTokenService = new Mock<ITokenService>();
            _controller = new AccountController(_mockUnitOfWork.Object, _mockTokenService.Object);
        }

        [Fact]
        public async Task Register_ReturnsOkResult_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "testuser",
                Password = "Password123",
                Role = UserRole.Customer
            };

            _mockUnitOfWork.Setup(uow => uow.Repository<User>().Add(It.IsAny<User>()))
                           .Callback<User>(user =>
                           {
                               user.UserId = 1; // Simulate setting the UserId upon saving
                           });
            _mockUnitOfWork.Setup(uow => uow.CompleteAsync()).ReturnsAsync(1); // Simulate one entity saved

            _mockTokenService.Setup(ts => ts.CreateToken(It.IsAny<User>())).Returns("fake.jwt.token");

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userDto = Assert.IsType<UserDto>(okResult.Value);
            Assert.Equal(registerDto.Username, userDto.Username);
            Assert.Equal("fake.jwt.token", userDto.Token);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                Username = "existinguser",
                Password = "Password123",
                Role = UserRole.Customer
            };

            _mockUnitOfWork.Setup(uow => uow.Repository<User>().GetAnyAsync(registerDto.Username)).ReturnsAsync(true);

            // Act
            var result = await _controller.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(badRequestResult.Value);
            Assert.Equal(404, apiResponse.StatusCode);
            Assert.Equal("Username already exist", apiResponse.Message);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WhenLoginIsSuccessful()
        {
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "InvalidPassword"
            };

            var user = new User
            {
                UserId = 1,
                UserName = loginDto.Username,
                PasswordHash = GetPasswordHash("Password123"),
                PasswordSalt = GetRandomBytes(64)
            };

            _mockUnitOfWork.Setup(uow => uow.Repository<User>().SingleOrDefaultAsync(u => u.UserName == loginDto.Username))
                           .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.Equal(401, apiResponse.StatusCode);
            Assert.Equal("Invalid Password", apiResponse.Message);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "nonexistinguser",
                Password = "Password123"
            };

            _mockUnitOfWork.Setup(uow => uow.Repository<User>().SingleOrDefaultAsync(u => u.UserName == loginDto.Username))
                           .ReturnsAsync((User)null);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.Equal(401, apiResponse.StatusCode);
            Assert.Equal("Invalid UserName", apiResponse.Message);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = "testuser",
                Password = "InvalidPassword"
            };

            var user = new User
            {
                UserId = 1,
                UserName = loginDto.Username,
                PasswordHash = GetPasswordHash("Password123"),
                PasswordSalt = GetRandomBytes(64)
            };

            _mockUnitOfWork.Setup(uow => uow.Repository<User>().SingleOrDefaultAsync(u => u.UserName == loginDto.Username))
                           .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result.Result);
            var apiResponse = Assert.IsType<ApiResponse>(unauthorizedResult.Value);
            Assert.Equal(401, apiResponse.StatusCode);
            Assert.Equal("Invalid Password", apiResponse.Message);
        }

        private byte[] GetPasswordHash(string password)
        {
            using var hmac = new HMACSHA512();
            return hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private byte[] GetRandomBytes(int length)
        {
            byte[] bytes = new byte[length];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }
    }
}
