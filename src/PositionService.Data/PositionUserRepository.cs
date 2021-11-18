using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Requests.Position;
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
      if (positionUser == null)
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

    public async Task<List<(DbPositionUser position, DbUserRate rate)>> GetAsync(IGetPositionsRequest request)
    {
      return (await
        (from positionUser in _provider.PositionsUsers
         join position in _provider.Positions on positionUser.PositionId equals position.Id
         join rate in _provider.UsersRates on positionUser.UserId equals rate.UserId
         where positionUser.IsActive && request.UsersIds.Contains(positionUser.UserId) && rate.IsActive
         select new
         {
           Position = position,
           PositionUser = positionUser,
           Rate = rate
         }).ToListAsync())
         .Select(data =>
         {
           data.PositionUser.Position = data.Position;

           return (data.PositionUser, data.Rate);
         }).ToList();
    }

    public async Task RemoveAsync(Guid userId, Guid removedBy)
    {
      DbPositionUser user = await _provider.PositionsUsers.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (user != null)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = DateTime.UtcNow;
        user.ModifiedBy = removedBy;
        await _provider.SaveAsync();
      }
    }
  }
}
