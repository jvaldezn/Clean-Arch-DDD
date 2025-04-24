using System.Data;
using AutoMapper;
using CleanArch.Application.Users;
using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Shared;
using CleanArch.Domain.Users;
using CleanArch.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Application.Tests
{
    public class UserServiceTests
    {
        private readonly IUnitOfWork<AppDbContext> _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork<AppDbContext>>();
            _mapper = Substitute.For<IMapper>();
            _configuration = Substitute.For<IConfiguration>();
            _userRepository = Substitute.For<IUserRepository>();
            _logger = Substitute.For<ILogger<UserService>>();
            _userService = new UserService(_unitOfWork, _mapper, _userRepository, _logger, _configuration);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsEmptyList()
        {
            _userRepository.GetAllAsync().Returns(new List<User>());

            var result = await _userService.GetAllUsers();

            Assert.True(result.IsSuccess);
            Assert.Equal(UserMessages.NoUsersFound, result.Message);
        }

        [Fact]
        public async Task GetUserById_ReturnsUser()
        {
            var user = new User { Id = 1, Username = "admin", Email = "admin@admin.com", Password = "Admin123!", DateOfBirth = new DateTime(1990, 1, 1), Role = UserRole.Admin };
            var userDto = new UserDto { Id = 1, Username = "admin", Email = "admin@admin.com", Password = "Admin123!", DateOfBirth = new DateTime(1990, 1, 1), Role = UserRole.Admin };

            _mapper.Map<UserDto>(user).Returns(userDto);
            _userRepository.GetByIdAsync(1).Returns(user);

            var result = await _userService.GetUserById(1);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(userDto.Username, result.Data?.Username);
        }

        [Fact]
        public async Task CreateUser_ReturnsSuccess()
        {
            var user = new User { Id = 1, Username = "admin", Email = "admin@admin.com", Password = "Admin123!", DateOfBirth = new DateTime(1990, 1, 1), Role = UserRole.Admin };
            var userDto = new UserDto { Id = 1, Username = "admin", Email = "admin@admin.com", Password = "Admin123!", DateOfBirth = new DateTime(1990, 1, 1), Role = UserRole.Admin };

            var transaction = Substitute.For<IDbContextTransaction>();

            _unitOfWork.BeginTransactionAsync().Returns(Task.FromResult(transaction));
            _mapper.Map<User>(userDto).Returns(user);
            _userRepository.AddAsync(user).Returns(Task.CompletedTask);
            _unitOfWork.SaveAsync().Returns(1);
            _mapper.Map<UserDto>(user).Returns(userDto);

            var result = await _userService.CreateUser(userDto);

            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.Id);
            Assert.Equal(UserMessages.UserCreated, result.Message);

            await _unitOfWork.Received(1).BeginTransactionAsync();
            await transaction.Received(1).CommitAsync(Arg.Any<CancellationToken>());
        }
    }
}
