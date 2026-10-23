using System.ComponentModel.DataAnnotations;

namespace InventoryMangmentSystem.DTOs.Auth;

public class RegisterDto
{
    [Required, MaxLength(50)]
    public string UserName { get; set; } = string.Empty;

    [Required, EmailAddress, MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(6)]
    public string Password { get; set; } = string.Empty;

    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    // Admin | Supplier | User
    [Required]
    public string Role { get; set; } = "User";

    // اختياري: لو المستخدم Supplier
    public Guid? SupplierId { get; set; }
}