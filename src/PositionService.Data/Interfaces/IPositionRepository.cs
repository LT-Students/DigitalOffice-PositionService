﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.PositionService.Data.Interfaces
{
  [AutoInject]
  public interface IPositionRepository
  {
    Task CreateAsync(DbPosition position);

    Task<DbPosition> GetAsync(Guid positionId);

    Task<List<DbPosition>> GetAsync(IGetPositionsRequest request);

    Task<List<DbPosition>> GetAsync(List<Guid> positionsIds);

    Task<(List<DbPosition>, int totalCount)> FindAsync(FindPositionsFilter filter);

    Task<bool> ContainsUsersAsync(Guid positionId);

    Task<bool> EditAsync(DbPosition position, JsonPatchDocument<DbPosition> request);

    Task<bool> DoesNameExistAsync(string name, Guid? positionId = null);

    Task<bool> DoesExistAsync(Guid positionId);
  }
}
