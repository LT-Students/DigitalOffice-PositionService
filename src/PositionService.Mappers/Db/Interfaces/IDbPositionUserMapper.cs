﻿using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;

namespace LT.DigitalOffice.PositionService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbPositionUserMapper
  {
    DbPositionUser Map(ICreateUserPositionPublish request);

    DbPositionUser Map(EditPositionUserRequest request);
  }
}
