using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Data.Provider;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.Kernel.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Requests.Position;

namespace LT.DigitalOffice.PositionService.Data
{
  public class PositionRepository : IPositionRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public PositionRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> CreateAsync(DbPosition newPosition)
    {
      if (newPosition == null)
      {
        return null;
      }

      _provider.Positions.Add(newPosition);
      await _provider.SaveAsync();

      return newPosition.Id;
    }

    public async Task<DbPosition> GetAsync(Guid positionId)
    {
      return await _provider.Positions.FirstOrDefaultAsync(d => d.Id == positionId);
    }

    public async Task<(List<DbPosition>, int totalCount)> FindAsync(FindPositionsFilter filter)
    {
      var dbPositions = _provider.Positions.AsQueryable();

      if (!filter.IncludeDeactivated)
      {
        dbPositions = dbPositions.Where(p => p.IsActive);
      }

      return (await dbPositions.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(), await dbPositions.CountAsync());
    }

    public async Task<List<DbPosition>> GetAsync(IGetPositionsRequest request)
    {
      if (request.UsersIds == null || !request.UsersIds.Any())
      {
        return null;
      }

      IQueryable<DbPositionUser> usersPositions = _provider.PositionsUsers
        .Where(u => u.IsActive && request.UsersIds.Contains(u.UserId))
        .Include(u => u.Position);

      return (await usersPositions.ToListAsync()).Select(pu => pu.Position).ToList();
    }

    public async Task<bool> ContainsUsersAsync(Guid positionId)
    {
      return await _provider.PositionsUsers
        .AnyAsync(pu => pu.PositionId == positionId && pu.IsActive);
    }

    public async Task<bool> EditAsync(DbPosition position, JsonPatchDocument<DbPosition> request)
    {
      if (position == null)
      {
        return false;
      }

      request.ApplyTo(position);
      position.ModifiedAtUtc = DateTime.UtcNow;
      position.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      await _provider.SaveAsync();

      return true;
    }

    public async Task<bool> DoesNameExistAsync(string name)
    {
      return await _provider.Positions.AnyAsync(p => p.Name == name);
    }

    public async Task<bool> DoesExistAsync(Guid positionId)
    {
      return await _provider.Positions.AnyAsync(p => p.Id == positionId);
    }
  }
}
