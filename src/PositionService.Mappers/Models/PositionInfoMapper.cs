using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;

namespace LT.DigitalOffice.PositionService.Mappers.Models
{
  public class PositionInfoMapper : IPositionInfoMapper
  {
    public PositionInfo Map(DbPosition position)
    {
      if (position == null)
      {
        return null;
      }

      return new PositionInfo
      {
        Id = position.Id,
        Name = position.Name,
        Description = position.Description,
        IsActive = position.IsActive
      };
    }
  }
}
