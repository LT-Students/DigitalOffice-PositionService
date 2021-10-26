using System;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Db
{
  public class DbPositionUserMapper : IDbPositionUserMapper
  {
    public DbPositionUser Map(Guid userId, Guid positionId, Guid modifiedBy)
    {
      return new DbPositionUser
      {
        Id = Guid.NewGuid(),
        UserId = userId,
        PositionId = positionId,
        IsActive = true,
        CreatedBy = modifiedBy,
        CreatedAtUtc = DateTime.UtcNow
      };
    }
  }
}
