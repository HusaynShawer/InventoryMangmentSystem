using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Exceptions;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using System.Security.Claims;
namespace InventoryMangmentSystem.Services;

public class GWarehouseService
{
    private readonly BaseRepository<Warehouse> _repo;
    private readonly WarehouseRepository _warehouseRepository;
    private readonly BaseRepository<UserWarehouse> _userWarehouseRepo;
    private readonly UnitOfWork _unitOfWork;
    public GWarehouseService(
        BaseRepository<Warehouse> repo,WarehouseRepository warehouseRepository,
        BaseRepository<UserWarehouse> userWarehouseRepository,
        UnitOfWork unitOfWork)
    {
        _repo = repo;
        _warehouseRepository = warehouseRepository;
        _userWarehouseRepo = userWarehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WarehouseResponseDTO>> GetAll()
    {
        var recods = await _repo.GetAll();
        return recods.Select(MapToDto).ToList();
    }
    public async Task<WarehouseResponseDTO> GetByID(Guid id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new NotFoundException("Warehouse not found.");
        return MapToDto(record);
    }

    public async Task<WarehouseResponseDTO> Create(Guid userId, WarehouseDTO warehouseDTO)
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
    public async Task Update(Guid id, WarehouseDTO warehouseDTO)
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

    public static WarehouseResponseDTO MapToDto(Warehouse w)=> new()
    {
        Id = w.Id,
        Location = w.Location,
        Name = w.Name
    };
    private async Task<Warehouse> GetEntityOrThrow(Guid id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new NotFoundException("Warehouse not found.");
        return record;
    }
 }