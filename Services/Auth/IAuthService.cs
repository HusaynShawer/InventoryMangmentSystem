using InventoryMangmentSystem.DTOs.Auth;

namespace InventoryMangmentSystem.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}