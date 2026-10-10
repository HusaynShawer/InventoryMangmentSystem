public class WarehouseDTO
{
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
}
public class WarehouseResponseDTO
{
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
}