public class StockDTO
{
    public Guid WarehouseId {get; set;}
    public required int Quantity {get; set;}

}

public class StockResponseDTO
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public int Quantity { get; set; }
}

public class StockMovementDTO
{
    public Guid ProductId{get; set;}
    public Guid WarehouseId {get; set;}
    public Guid UserId {get; set;}
    public int Quantity{get; set;}
    public int MovementType {get; set;}
    public Guid ReferenceId{get;set;}
    public DateTime Date { get; set;}
    public string? Note {get; set;}
}

public class StockAdjustmentDTO
{
    public Guid ProductId {get; set;}
    public Guid WarehouseId{get; set;}
    public string reason {get; set;}
    public int ActualQuantity {get; set;}
}