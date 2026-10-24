using InventoryManagementSystem.Models;

namespace InventoryManagementSystem.Services.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
}