using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Mappers.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Data
{
  public class PositionUserDataMapper : IPositionUserDataMapper
  {
    public PositionUserData Map(DbPositionUser user)
    {
      if (user is null)
      {
        return null;
      }

      return new(user.UserId, user.CreatedAtUtc);
    }
  }
}
