using InventoryMangmentSystem.Models;
using InventoryMangmentSystem.Data;
using InventoryMangmentSystem.Repositories;
using System.Security.Authentication;
using Microsoft.AspNetCore.Authentication;
namespace InventoryMangmentSystem.Services;

public class GStockService
{
    private readonly BaseRepository<Stock> _repo;

    public GStockService(
        BaseRepository<Stock> repo)
    {
        _repo = repo;
    }

    public async Task<Stock> Create(Stock stock)
    {
        return await _repo.Add(stock);
    }
}