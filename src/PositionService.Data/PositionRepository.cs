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

    public Task CreateAsync(DbPosition newPosition)
    {
      if (newPosition is null)
      {
        return Task.CompletedTask;
      }

      _provider.Positions.Add(newPosition);
      return _provider.SaveAsync();
    }

    public Task<DbPosition> GetAsync(Guid positionId)
    {
      return _provider.Positions.FirstOrDefaultAsync(d => d.Id == positionId);
    }

    public async Task<(List<DbPosition>, int totalCount)> FindAsync(FindPositionsFilter filter)
    {
      IQueryable<DbPosition> positionQuery = _provider.Positions;

      if (!filter.IncludeDeactivated)
      {
        positionQuery = positionQuery.Where(x => x.IsActive);
      }

      if (filter.IsAscendingSort.HasValue)
      {
        positionQuery = filter.IsAscendingSort.Value
          ? positionQuery.OrderBy(o => o.Name)
          : positionQuery.OrderByDescending(o => o.Name);
      }

      if (!string.IsNullOrWhiteSpace(filter.NameIncludeSubstring))
      {
        positionQuery = positionQuery.Where(d => d.Name.ToLower().Contains(filter.NameIncludeSubstring.ToLower()));
      }

      return (await positionQuery.Skip(filter.SkipCount).Take(filter.TakeCount).ToListAsync(), await positionQuery.CountAsync());
    }

    public Task<List<DbPosition>> GetAsync(IGetPositionsRequest request)
    {
      IQueryable<DbPosition> positionQuery = _provider.Positions.AsQueryable();

      if (request.UsersIds is not null && request.UsersIds.Any())
      {
        positionQuery = positionQuery
          .Where(d =>
            d.IsActive
            && d.Users.Any(du => du.IsActive && request.UsersIds.Contains(du.UserId)));
      }

      positionQuery = positionQuery.Include(d => d.Users.Where(du => du.IsActive));

      return positionQuery.ToListAsync();
    }

    public Task<List<DbPosition>> GetAsync(List<Guid> positionsIds)
    {
      return _provider.Positions
        .Where(
          p => positionsIds.Contains(p.Id)).Include(p => p.Users.Where(u => u.IsActive))
        .ToListAsync();
    }

    public Task<bool> ContainsUsersAsync(Guid positionId)
    {
      return _provider.PositionsUsers
        .AnyAsync(pu => pu.PositionId == positionId && pu.IsActive);
    }

    public async Task<bool> EditAsync(DbPosition position, JsonPatchDocument<DbPosition> request)
    {
      if (position is null)
      {
        return false;
      }

      request.ApplyTo(position);
      position.ModifiedAtUtc = DateTime.UtcNow;
      position.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      await _provider.SaveAsync();

      return true;
    }

    public Task<bool> DoesNameExistAsync(string name, Guid? positionId = null)
    {
      return positionId.HasValue
        ? _provider.Positions.AnyAsync(p => p.Name == name && p.Id != positionId)
        : _provider.Positions.AnyAsync(p => p.Name == name);
    }

    public Task<bool> DoesExistAsync(Guid positionId)
    {
      return _provider.Positions.AnyAsync(p => p.Id == positionId);
    }
  }
}
