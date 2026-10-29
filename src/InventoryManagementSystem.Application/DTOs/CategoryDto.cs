namespace InventoryManagementSystem.Application.DTOs;

public class CategoryDto
{
    public string Name { get; set; } = string.Empty;
}

public class CategoryResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
