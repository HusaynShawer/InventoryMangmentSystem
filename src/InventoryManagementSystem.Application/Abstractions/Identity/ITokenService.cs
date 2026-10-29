using InventoryManagementSystem.Domain.Entities;

namespace InventoryManagementSystem.Application.Abstractions.Identity;

public interface ITokenService
{
    string GenerateToken(User user);
}
