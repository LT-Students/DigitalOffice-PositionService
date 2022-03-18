using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.DataSupport.Database.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
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

      _provider.Add(positionUser);
      await _provider.SaveAsync();

      return positionUser.Id;
    }

    public async Task<DbPositionUser> GetAsync(Guid userId)
    {
      return await _provider.Get<DbPositionUser>().Include(u => u.Position).FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
    }

    public async Task<List<DbPositionUser>> GetAsync(List<Guid> userIds)
    {
      return await _provider.Get<DbPositionUser>()
        .Include(pu => pu.Position)
        .Where(u => userIds.Contains(u.UserId) && u.IsActive)
        .ToListAsync();
    }

    public async Task<Guid?> RemoveAsync(Guid userId, Guid removedBy)
    {
      DbPositionUser dbPositionUser = await _provider.Get<DbPositionUser>()
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
