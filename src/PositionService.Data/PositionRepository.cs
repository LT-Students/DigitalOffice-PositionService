using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Data.Provider;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

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
      IQueryable<DbPosition> dbPositions = _provider.Positions.AsQueryable();

      if (!filter.IncludeDeactivated)
      {
        dbPositions = dbPositions.Where(p => p.IsActive);
      }

      return (await dbPositions.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(), await dbPositions.CountAsync());
    }

    public async Task<List<DbPosition>> GetAsync(IGetPositionsRequest request)
    {
      IQueryable<DbPosition> dbPosition = _provider.Positions.AsQueryable();

      if (request.UsersIds is not null && request.UsersIds.Any())
      {
        dbPosition = dbPosition
          .Where(d => 
            d.IsActive
            && d.Users.Any(du => du.IsActive && request.UsersIds.Contains(du.UserId)));
      }

      dbPosition = dbPosition.Include(d => d.Users.Where(du => du.IsActive));

      return await dbPosition.ToListAsync();
    }

    public async Task<List<DbPosition>> GetAsync(List<Guid> positionsIds)
    {
      return await _provider.Positions.Where(
        p => positionsIds.Contains(p.Id)).Include(p => p.Users.Where(u => u.IsActive))
        .ToListAsync();
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
