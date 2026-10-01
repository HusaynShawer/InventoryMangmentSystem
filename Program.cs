using System.Text;
using Microsoft.EntityFrameworkCore;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Services;
using InventoryMangmentSystem.Repositories;
using InventoryMangmentSystem.Services.Auth;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllers();

// 1. Register the DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Repositories
// builder.Services.AddScoped<IProductRepository, ProductRepository>();
// builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IStockRepository, StockRepository>();
// builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
// builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
// builder.Services.AddScoped<IPurchaseItemsRepository, PurchaseItemsRepository>();
// builder.Services.AddScoped<ISaleRepository, SaleRepository>();
// builder.Services.AddScoped<ISaleItemRepository, SaleItemRepository>();
// builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
// builder.Services.AddScoped<IWarehouseRepository, WarehouseRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserWarehouseRepository, UserWarehouseRepository>();
// Services

builder.Services.AddScoped(typeof(BaseRepository<>));
builder.Services.AddScoped<GProductService>();
builder.Services.AddScoped<GCategoryService>();
//builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<GSupplierService>();
builder.Services.AddScoped<GPurchaseService>();
builder.Services.AddScoped<GPurchaseItemsService>();
builder.Services.AddScoped<GsaleService>();
//builder.Services.AddScoped<ISaleItemService, SaleItemService>();
builder.Services.AddScoped<GStockMovementService>();
builder.Services.AddScoped<GWarehouseService>();
builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddOpenApi();

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ClockSkew = TimeSpan.Zero
    
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", p => p.RequireRole("Admin"));
    options.AddPolicy("SupplierOnly", p => p.RequireRole("Supplier"));
    options.AddPolicy("UserOnly", p => p.RequireRole("User"));
});



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();