using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;

namespace LT.DigitalOffice.PositionService.Mappers.Models.Interfaces
{
  [AutoInject]
  public interface IPositionInfoMapper
  {
    PositionInfo Map(DbPosition position);
  }
}
