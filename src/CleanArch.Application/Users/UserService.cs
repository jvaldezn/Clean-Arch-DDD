using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using CleanArch.Domain.Abstractions;
using CleanArch.Domain.Users;
using CleanArch.Domain.Shared;
using CleanArch.Infrastructure.Helper;
using CleanArch.Infrastructure;
using CleanArch.Infrastructure.Context;

namespace CleanArch.Application.Users;

public class UserService : IUserService
{
    private readonly IUnitOfWork<AppDbContext> _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IConfiguration _configuration;

    public UserService(IUnitOfWork<AppDbContext> unitOfWork, IMapper mapper, IUserRepository userRepository, ILogger<UserService> logger, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userRepository = userRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<Response<string>> AuthenticateUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return Response<string>.Failure(UserMessages.InvalidCredentials);

        var user = await _userRepository.GetByUsernameAsync(username);

        if (user == null || !PasswordHelper.VerifyPassword(password, user.Password!))
            return Response<string>.Failure(UserMessages.InvalidCredentials);

        var token = GenerateJwtToken(user);
        return Response<string>.Success(token);
    }

    public async Task<Response<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userRepository.GetAllAsync();
        var userDtos = _mapper.Map<IEnumerable<UserDto>>(users);

        if (!userDtos.Any())
            return Response<IEnumerable<UserDto>>.Success(userDtos, UserMessages.NoUsersFound);

        return Response<IEnumerable<UserDto>>.Success(userDtos);
    }

    public async Task<Response<UserDto>> GetUserById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user == null
            ? Response<UserDto>.Failure(UserMessages.UserNotFound)
            : Response<UserDto>.Success(_mapper.Map<UserDto>(user));
    }

    public async Task<Response<UserDto>> CreateUser(UserDto dto)
    {
        var validator = await new UserDtoValidator().ValidateAsync(dto);
        if (!validator.IsValid)
            return Response<UserDto>.Failure(validator.Errors.GetErrorMessage());

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            if (await _userRepository.GetByUsernameAsync(dto.Username!) != null)
                return Response<UserDto>.Failure(UserMessages.UsernameAlreadyExists);
            if (await _userRepository.GetByEmailAsync(dto.Email!) != null)
                return Response<UserDto>.Failure(UserMessages.EmailAlreadyExists);

            var user = _mapper.Map<User>(dto);
            user.Password = PasswordHelper.HashPassword(dto.Password!);

            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return Response<UserDto>.Success(_mapper.Map<UserDto>(user), UserMessages.UserCreated);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(string.Format(Messages.UnexpectedError, ex.Message), UserMessages.UserCreatedError);
            return Response<UserDto>.Failure(string.Format(Messages.UnexpectedError, ex.Message));
        }
    }

    public async Task<Response<UserDto>> UpdateUser(int id, UserDto dto)
    {
        if (id != dto.Id)
            return Response<UserDto>.Failure(UserMessages.UserNoMatch);

        var validator = await new UserDtoValidator().ValidateAsync(dto);
        if (!validator.IsValid)
            return Response<UserDto>.Failure(validator.Errors.GetErrorMessage());

        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _userRepository.GetByIdAsync(dto.Id);
            if (user == null)
                return Response<UserDto>.Failure(UserMessages.UserNotFound);

            _mapper.Map(dto, user);
            user.Password = PasswordHelper.HashPassword(dto.Password!);

            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return Response<UserDto>.Success(_mapper.Map<UserDto>(user), UserMessages.UserUpdated);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(string.Format(Messages.UnexpectedError, ex.Message), string.Format(UserMessages.UserUpdatedError, id));
            return Response<UserDto>.Failure(string.Format(Messages.UnexpectedError, ex.Message));
        }
    }

    public async Task<Response<bool>> DeleteUser(int id)
    {
        await using var transaction = await _unitOfWork.BeginTransactionAsync();
        try
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                return Response<bool>.Failure(UserMessages.UserNotFound);

            _userRepository.Delete(user);
            await _unitOfWork.SaveAsync();
            await transaction.CommitAsync();

            return Response<bool>.Success(true, UserMessages.UserDeleted);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(string.Format(Messages.UnexpectedError, ex.Message), string.Format(UserMessages.UserDeletedError, id));
            return Response<bool>.Failure(string.Format(Messages.UnexpectedError, ex.Message));
        }
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = _configuration.GetValue<string>(Constant.JwtKey) ?? throw new InvalidOperationException();
        var issuer = _configuration.GetValue<string>(Constant.Issuer) ?? throw new InvalidOperationException();
        var audience = _configuration.GetValue<string>(Constant.Audience) ?? throw new InvalidOperationException();

        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.UniqueName, user.Username!),
                    new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString())
                }),
            Issuer = issuer,
            Audience = audience,
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = creds
        };

        var token = jwtTokenHandler.CreateToken(tokenDescriptor);
        var jwtToken = jwtTokenHandler.WriteToken(token);

        return jwtToken;
    }
}