using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Repositories;
using System.Security.Claims;
namespace InventoryMangmentSystem.Services;

public class GWarehouseService
{
    private readonly BaseRepository<Warehouse> _repo;
    private readonly WarehouseRepository _warehouseRepository;
    private readonly UnitOfWork _unitOfWork;
    public GWarehouseService(
        BaseRepository<Warehouse> repo,WarehouseRepository warehouseRepository,UnitOfWork unitOfWork)
    {
        _repo = repo;
        _warehouseRepository = warehouseRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<WarehouseResponseDTO>> GetAll()
    {
        var recods =  await _repo.GetAll();
        if (!recods.Any())
            throw new Exception("No Warehouse found");
        return recods.Select(MapToDto).ToList();
    }
    public async Task<WarehouseResponseDTO> GetByID(Guid id)
    {
        var record = await _repo.GetById(id);
        if (record is null)
            throw new Exception("Warehouse Not found");
        return MapToDto(record);
    }

    public async Task<WarehouseResponseDTO> Create(WarehouseDTO warehouseDTO)
    {

        var recod = await _repo.Add(new Warehouse{Name = warehouseDTO.Name,
                                    Location = warehouseDTO.Location});
        if (recod is null)
            throw new Exception("sorry Warehouse item dont added try again");
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
        var records = await _warehouseRepository.GetWarehouseLowProducts(userId);
        return records.Any() ? records : throw new Exception("no low products in warehouse");
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
            throw new KeyNotFoundException("Product not found");
        return record;
    }
 }