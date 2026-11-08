namespace InventoryManagementSystem.Domain.Enums;

public enum CartStatus
{
    Active = 1,
    Converted = 2,
    Abandoned = 3
}

public enum OrderStatus
{
    Pending = 1,     
    Reserved = 2,    
    Paid = 3,        
    Shipped = 4,     
    Delivered = 5,   
    Cancelled = 6    
}

public enum OrderItemStatus
{
    Active = 1,
    Cancelled = 2
}

public enum StockMovementType
{
    PurchaseIn = 1,     
    SaleOut = 2,        
    Reservation = 3,    
    Release = 4,        
    Transfer = 5,       
    Adjustment = 6,
    TransferIn = 7,
    TransferOut = 8
}

public enum StockReservationStatus
{
    Active = 1,
    Released = 2,       
    Consumed = 3        
}