namespace InventoryManagementSystem.DTOs;

public class ProductAttributeDto
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class ProductAttributeResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public List<ProductAttributeValueResponseDto> Values { get; set; } = new();
}

public class ProductAttributeValueDto
{
    public string Value { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

public class ProductAttributeValueResponseDto
{
    public Guid Id { get; set; }
    public Guid ProductAttributeId { get; set; }
    public string Value { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
