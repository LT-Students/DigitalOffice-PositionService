using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Mappers.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Data
{
  public class PositionUserDataMapper : IPositionUserDataMapper
  {
    public PositionUserData Map(DbPositionUser user, DbUserRate rate)
    {
      if (user == null)
      {
        return null;
      }

      return new(user.UserId, rate.Rate, user.CreatedAtUtc);
    }
  }
}
