using CleanArch.Domain.Users;

namespace CleanArch.Application.Users;

public class UserDto
{
    public int Id { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public DateTime DateOfBirth { get; set; }
    public UserRole Role { get; set; }
}
