using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbUserRateMapper
  {
    DbUserRate Map(ICreateUserPositionRequest request);
  }
}
