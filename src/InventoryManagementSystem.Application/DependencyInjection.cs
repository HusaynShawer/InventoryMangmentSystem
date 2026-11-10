using InventoryManagementSystem.Application.Services;
using InventoryManagementSystem.Application.Services.Auth;
using Microsoft.Extensions.DependencyInjection;

namespace InventoryManagementSystem.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<CategoryService>();
        services.AddScoped<ProductService>();
        services.AddScoped<ProductVariantService>();
        services.AddScoped<ProductAttributeService>();
        services.AddScoped<StockService>();
        services.AddScoped<StokLevelService>();
        services.AddScoped<SupplierService>();
        services.AddScoped<PurchaseService>();
        services.AddScoped<PurchaseItemService>();
        services.AddScoped<SaleService>();
        services.AddScoped<SaleItemService>();
        services.AddScoped<StockMovementService>();
        services.AddScoped<UserWarehouseService>();
        services.AddScoped<WarehouseService>();
        services.AddScoped<CartService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<OrderService>();

        return services;
    }
}
