using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
namespace InventoryMangmentSystem.Services;

public class GStockService
{
    private readonly BaseRepository<Stock> _repo;
    private readonly UnitOfWork _uow;
    public GStockService(
        BaseRepository<Stock> repo,UnitOfWork uow)
    {
        _repo = repo;
        _uow = uow;
    }

    public async Task<Stock> Create(Stock stock)
    {
        var result =  await _repo.Add(stock);
        await _uow.SaveAsync();
        return result;
    }
}