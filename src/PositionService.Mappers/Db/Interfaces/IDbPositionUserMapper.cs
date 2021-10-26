﻿using System;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Mappers.Db.Interfaces
{
  [AutoInject]
  public interface IDbPositionUserMapper
  {
    DbPositionUser Map(Guid userId, Guid positionId, Guid modifiedBy);
  }
}
