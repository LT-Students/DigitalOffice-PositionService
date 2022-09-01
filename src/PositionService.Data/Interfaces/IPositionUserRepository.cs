using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Data.Interfaces
{
  [AutoInject]
  public interface IPositionUserRepository
  {
    Task CreateAsync(DbPositionUser positionUser);

    Task<Guid?> ActivateAsync(IActivateUserPublish request);

    Task<DbPositionUser> GetAsync(Guid userId);

    Task<List<DbPositionUser>> GetAsync(List<Guid> userIds);

    Task<Guid?> EditAsync(Guid userId, Guid positionId);

    Task<Guid?> RemoveAsync(Guid userId, Guid removedBy);

    Task<bool> DoesExistAsync(Guid userId);
  }
}
