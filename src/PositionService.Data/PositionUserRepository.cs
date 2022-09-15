using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Publishing;
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

    public Task CreateAsync(DbPositionUser positionUser)
    {
      if (positionUser is null)
      {
        return null;
      }

      _provider.PositionsUsers.Add(positionUser);
      return _provider.SaveAsync();
    }

    public async Task<Guid?> ActivateAsync(IActivateUserPublish request)
    {
      DbPositionUser user = await _provider.PositionsUsers.FirstOrDefaultAsync(u => u.UserId == request.UserId && !u.IsActive);

      if (user is null)
      {
        return null;
      }

      user.IsActive = true;
      await _provider.SaveAsync();

      return user.PositionId;
    }

    public Task<DbPositionUser> GetAsync(Guid userId)
    {
      return _provider.PositionsUsers
        .Include(u => u.Position)
        .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
    }

    public Task<List<DbPositionUser>> GetAsync(List<Guid> userIds)
    {
      return _provider.PositionsUsers
        .Include(pu => pu.Position)
        .Where(u => userIds.Contains(u.UserId) && u.IsActive)
        .ToListAsync();
    }

    public async Task<Guid?> EditAsync(Guid userId, Guid positionId)
    {
      DbPositionUser dbPositionUser = await _provider.PositionsUsers
        .FirstOrDefaultAsync(u => u.UserId == userId);

      if (dbPositionUser is null)
      {
        return null;
      }

      if (!dbPositionUser.IsActive)
      {
        dbPositionUser.PositionId = positionId;
        dbPositionUser.IsActive = true;
      }
      else
      {
        dbPositionUser.PositionId = positionId;
      }

      await _provider.SaveAsync();
      return dbPositionUser.PositionId;
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
      dbPositionUser.CreatedBy = removedBy;
      await _provider.SaveAsync();

      return dbPositionUser.PositionId;
    }

    public Task<bool> DoesExistAsync(Guid userId)
    {
      return _provider.PositionsUsers.AnyAsync(pu => pu.UserId == userId);
    }
  }
}
