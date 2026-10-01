using InventoryMangmentSystem.Models;

namespace InventoryMangmentSystem.Services.Auth;

public interface ITokenService
{
    string GenerateToken(User user);
}