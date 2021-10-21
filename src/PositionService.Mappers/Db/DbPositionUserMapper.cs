using System;
using LT.DigitalOffice.Models.Broker.Requests.Position;
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

    public DbPositionUser Map(IEditUserPositionRequest request)
    {
      if (request == null)
      {
        return null;
      }

      return new DbPositionUser
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        PositionId = request.PositionId,
        IsActive = true,
        CreatedBy = request.ModifiedBy,
        CreatedAtUtc = DateTime.UtcNow
      };
    }
  }
}
