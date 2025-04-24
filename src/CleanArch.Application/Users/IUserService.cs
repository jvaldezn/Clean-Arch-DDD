using CleanArch.Domain.Abstractions;

namespace CleanArch.Application.Users;

public interface IUserService
{
    Task<Response<string>> AuthenticateUser(string username, string password);
    Task<Response<IEnumerable<UserDto>>> GetAllUsers();
    Task<Response<UserDto>> GetUserById(int id);
    Task<Response<UserDto>> CreateUser(UserDto dto);
    Task<Response<UserDto>> UpdateUser(int id, UserDto dto);
    Task<Response<bool>> DeleteUser(int id);
}
