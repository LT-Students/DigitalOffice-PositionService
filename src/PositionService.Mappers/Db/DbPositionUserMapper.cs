using System;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Db
{
  public class DbPositionUserMapper : IDbPositionUserMapper
  {
    public DbPositionUser Map(ICreateUserPositionRequest request)
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
        Rate = request.Rate,
        IsActive = true,
        CreatedBy = request.CreatedBy,
        CreatedAtUtc = DateTime.UtcNow
      };
    }
  }
}
