using InventoryManagementSystem.DTOs.Auth;

namespace InventoryManagementSystem.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}