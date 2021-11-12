using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate;

namespace LT.DigitalOffice.PositionService.Data.Interfaces
{
  [AutoInject]
  public interface IUserRateRepository
  {
    Task<Guid?> CreateAsync(DbUserRate dbUserRate);
    Task<bool> EditAsync(EditUserRateRequest request);
    Task<bool> DoesExistAsync(Guid userId);
    Task RemoveAsync(Guid userId, Guid removedBy);
  }
}
