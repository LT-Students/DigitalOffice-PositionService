using System;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Db
{
  public class DbUserRateMapper : IDbUserRateMapper
  {
    public DbUserRate Map(ICreateUserPositionRequest request)
    {
      return new DbUserRate
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        Rate = request.Rate,
        CreatedBy = request.CreatedBy,
        CreatedAtUtc = DateTime.UtcNow,
        IsActive = true
      };
    }
  }
}
