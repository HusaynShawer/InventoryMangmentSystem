using InventoryMangmentSystem.Exceptions;

namespace InventoryMangmentSystem.Services.Auth;

public static class AppRoles
{
    public const string Admin = "ADMIN";
    public const string Supplier = "Supplier";
    public const string User = "User";

    public static bool TryNormalize(string? role, out string normalized)
    {
        switch (role?.Trim().ToUpperInvariant())
        {
            case "ADMIN":
                normalized = Admin;
                return true;
            case "SUPPLIER":
                normalized = Supplier;
                return true;
            case "USER":
                normalized = User;
                return true;
            default:
                normalized = string.Empty;
                return false;
        }
    }

    public static string NormalizeOrThrow(string? role)
    {
        if (!TryNormalize(role, out var normalized))
            throw new BadRequestException(
                "Role must be one of: ADMIN, Supplier, User.");

        return normalized;
    }
}
