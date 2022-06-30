using System.Linq;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Mappers.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Data
{
  public class PositionDataMapper : IPositionDataMapper
  {
    public PositionData Map(DbPosition position)
    {
      if (position == null)
      {
        return null;
      }

      return new PositionData(
        position.Id,
        position.Name,
        position.Users.Select(user => user.UserId).ToList());
    }
  }
}
