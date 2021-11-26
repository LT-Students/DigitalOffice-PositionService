using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;

namespace LT.DigitalOffice.PositionService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbPositionMapper
  {
    DbPosition Map(CreatePositionRequest position);
  }
}
