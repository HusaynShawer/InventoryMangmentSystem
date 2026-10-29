# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["InventoryManagementSystem.slnx", "./"]
COPY ["src/InventoryManagementSystem.Domain/InventoryManagementSystem.Domain.csproj", "src/InventoryManagementSystem.Domain/"]
COPY ["src/InventoryManagementSystem.Application/InventoryManagementSystem.Application.csproj", "src/InventoryManagementSystem.Application/"]
COPY ["src/InventoryManagementSystem.Infrastructure/InventoryManagementSystem.Infrastructure.csproj", "src/InventoryManagementSystem.Infrastructure/"]
COPY ["src/InventoryManagementSystem.Api/InventoryManagementSystem.Api.csproj", "src/InventoryManagementSystem.Api/"]

RUN dotnet restore "src/InventoryManagementSystem.Api/InventoryManagementSystem.Api.csproj"

COPY . .

WORKDIR "/src/src/InventoryManagementSystem.Api"
RUN dotnet publish "InventoryManagementSystem.Api.csproj" -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "InventoryManagementSystem.Api.dll"]
