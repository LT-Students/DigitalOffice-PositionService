using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Data.Interfaces
{
  [AutoInject]
  public interface IPositionUserDataMapper
  {
    PositionUserData Map(DbPositionUser user);
  }
}
