using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Data.Provider;
using LT.DigitalOffice.PositionService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.PositionService.Data
{
  public class PositionUserRepository : IPositionUserRepository
  {
    private readonly IDataProvider _provider;

    public PositionUserRepository(
      IDataProvider provider)
    {
      _provider = provider;
    }

    public async Task<Guid?> CreateAsync(DbPositionUser positionUser)
    {
      if (positionUser is null)
      {
        return null;
      }

      _provider.PositionsUsers.Add(positionUser);
      await _provider.SaveAsync();

      return positionUser.Id;
    }

    public async Task<DbPositionUser> GetAsync(Guid userId)
    {
      return await _provider.PositionsUsers.Include(u => u.Position).FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
    }

    public async Task<List<DbPositionUser>> GetAsync(List<Guid> userIds)
    {
      return await _provider.PositionsUsers
        .Include(pu => pu.Position)
        .Where(u => userIds.Contains(u.UserId) && u.IsActive)
        .ToListAsync();
    }

    public async Task<Guid?> RemoveAsync(Guid userId, Guid removedBy)
    {
      DbPositionUser dbPositionUser = await _provider.PositionsUsers
        .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (dbPositionUser is null)
      {
        return null;
      }

      dbPositionUser.IsActive = false;
      dbPositionUser.ModifiedAtUtc = DateTime.UtcNow;
      dbPositionUser.ModifiedBy = removedBy;
      await _provider.SaveAsync();

      return dbPositionUser.PositionId;
    }
  }
}
