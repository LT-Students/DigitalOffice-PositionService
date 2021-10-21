using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Data.Interfaces
{
  [AutoInject]
  public interface IPositionUserRepository
  {
    Task<Guid?> CreateAsync(DbPositionUser positionUser);

    Task<DbPositionUser> GetAsync(Guid userId);

    Task<List<DbPositionUser>> GetAsync(List<Guid> userIds);

    Task RemoveAsync(Guid userId, Guid removedBy);
  }
}
