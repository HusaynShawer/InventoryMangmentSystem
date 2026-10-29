using InventoryManagementSystem.Application.DTOs.Auth;

namespace InventoryManagementSystem.Application.Services.Auth;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto dto);
    Task<AuthResponseDto> LoginAsync(LoginDto dto);
}