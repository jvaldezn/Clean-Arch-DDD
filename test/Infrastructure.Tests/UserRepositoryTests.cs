using CleanArch.Domain.Users;
using CleanArch.Infrastructure.Context;
using CleanArch.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Tests
{
    public class UserRepositoryTests
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserRepository _repository;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            _dbContext = new AppDbContext(options);
            _repository = new UserRepository(_dbContext);
        }

        [Fact]
        public async Task AddUser_Success()
        {
            var user = new User { Username = "admin", Email = "admin@admin.com", Password = "$2a$11$0uZGUz4TAmUdSl6XdvtIjeUIL/v7IiJujaOvcBxDn0tJnXvsmsIwi", DateOfBirth = new DateTime(1990, 1, 1), Role = UserRole.Admin };
            await _repository.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetByIdAsync(user.Id);

            Assert.NotNull(result);
            Assert.Equal("admin", result.Username);
        }

        [Fact]
        public async Task GetAllUser_ReturnsList()
        {
            _dbContext.User.Add(new User { Username = "admin", Email = "admin@admin.com", Password = "$2a$11$0uZGUz4TAmUdSl6XdvtIjeUIL/v7IiJujaOvcBxDn0tJnXvsmsIwi", DateOfBirth = new DateTime(1990, 1, 1), Role = UserRole.Admin });
            _dbContext.User.Add(new User { Username = "auditor", Email = "auditor@auditor.com", Password = "$2a$11$0uZGUz4TAmUdSl6XdvtIjeUIL/v7IiJujaOvcBxDn0tJnXvsmsIwi", DateOfBirth = new DateTime(1990, 1, 1), Role = UserRole.Guest });
            await _dbContext.SaveChangesAsync();

            var result = await _repository.GetAllAsync();

            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count());
        }
    }
}
