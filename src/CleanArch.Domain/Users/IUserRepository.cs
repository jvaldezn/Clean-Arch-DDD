using CleanArch.Domain.Abstractions;

namespace CleanArch.Domain.Users;

public interface IUserRepository : IGenericRepository<User>
{
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
}
