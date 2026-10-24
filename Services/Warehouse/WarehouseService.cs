using InventoryManagementSystem.Data;
using InventoryManagementSystem.DTOs;
using InventoryManagementSystem.Exceptions;
using InventoryManagementSystem.Models;
using InventoryManagementSystem.Repositories;
using System.Security.Claims;
namespace InventoryManagementSystem.Services;

public class WarehouseService
{
    private readonly BaseRepository<Warehouse> _repo;
    private readonly WarehouseRepository _warehouseRepository;
    private readonly BaseRepository<UserWarehouse> _userWarehouseRepo;
    private readonly UnitOfWork _unitOfWork;
    public WarehouseService(
        BaseRepository<Warehouse> repo,WarehouseRepository warehouseRepository,
        BaseRepository<UserWarehouse> userWarehouseRepository,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _warehouseRepository = warehouseRepository;
        _userWarehouseRepo = userWarehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WarehouseResponseDto>> GetAllAsync()
    {
        var recods = await _repo.GetAllAsync();
        return recods.Select(MapToDto).ToList();
    }
    public async Task<WarehouseResponseDto> GetByIdAsync(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Warehouse not found.");
        return MapToDto(record);
    }

    public async Task<WarehouseResponseDto> Create(Guid userId, WarehouseDto warehouseDTO)
    {

        var recod = await _repo.Add(new Warehouse{Name = warehouseDTO.Name,
                                    Location = warehouseDTO.Location});
        if (recod is null)
            throw new InvalidOperationException(
                "Warehouse could not be created.");
        var userwarehouse = new UserWarehouse
        {
            UserId=userId,
            WarehouseId = recod.Id,
            Warehouse = recod
        };
        await _userWarehouseRepo.Add(userwarehouse);
        await _unitOfWork.SaveAsync();
        return MapToDto(recod);

    }
    public async Task Delete(Guid id)
    {
        var record = await GetEntityOrThrow(id);
        _repo.Delete(record);
        await _unitOfWork.SaveAsync();
    }
    public async Task Update(Guid id, WarehouseDto warehouseDTO)
    {
        var record = await GetEntityOrThrow(id);  
        record.Location = warehouseDTO.Location;
        record.Name = warehouseDTO.Name;
        _repo.Update(record);
        await _unitOfWork.SaveAsync();
    }

    public async Task<IEnumerable<Product>> GetWarehouseProducts(Guid userId)
    {
        var records = await _warehouseRepository.GetWarehouseProduct(userId);
        return records;
    }
    public async Task<IEnumerable<Product>> GetWarehouseLowProducts(Guid userId)
    {
        return await _warehouseRepository.GetWarehouseLowProducts(userId);
    }

    public static WarehouseResponseDto MapToDto(Warehouse w)=> new()
    {
        Id = w.Id,
        Location = w.Location,
        Name = w.Name
    };
    private async Task<Warehouse> GetEntityOrThrow(Guid id)
    {
        var record = await _repo.GetByIdAsync(id);
        if (record is null)
            throw new NotFoundException("Warehouse not found.");
        return record;
    }
 }