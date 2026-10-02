using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.DTOs.Auth;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;

namespace InventoryMangmentSystem.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _hasher;
    private readonly ITokenService _tokenService;
    private readonly UnitOfWork _unitofWork;
    public AuthService(
        IUserRepository userRepo,
        IPasswordHasher hasher,
        ITokenService tokenService,
        UnitOfWork unitOfWork )
    {
        _userRepo = userRepo;
        _hasher = hasher;
        _tokenService = tokenService;
        _unitofWork = unitOfWork;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        
        var existing = await _userRepo.GetByUserNameAsync(dto.UserName);
        if (existing is not null)
            throw new Exception("UserName already exists");


        var existingEmail = await _userRepo.GetByEmailAsync(dto.Email);
        if (existingEmail is not null)
            throw new Exception("Email already exists");


        var user = new User
        {
            UserName = dto.UserName,
            Email = dto.Email,
            PasswordHash = _hasher.HashPassword(dto.Password),
            FullName = dto.FullName,
            Role = dto.Role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            SupplierId = dto.SupplierId
        };

        await _userRepo.CreateAsync(user);
        await _unitofWork.SaveAsync();
        var token = _tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserName = user.UserName,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _userRepo.GetByUserNameAsync(dto.UserName);
        if (user is null)
            throw new Exception("Invalid username or password");

        if (!user.IsActive)
            throw new Exception("Account is disabled");

        if (!_hasher.VerifyPassword(dto.Password, user.PasswordHash))
            throw new Exception("Invalid username or password");

        var token = _tokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            UserName = user.UserName,
            Role = user.Role,
            ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        };
    }
}