using System;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.PositionService.Mappers.Db
{
  public class DbPositionUserMapper : IDbPositionUserMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbPositionUserMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbPositionUser Map(ICreateUserPositionPublish request)
    {
      return request is null
        ? null
        : new DbPositionUser
        {
          Id = Guid.NewGuid(),
          UserId = request.UserId,
          PositionId = request.PositionId,
          IsActive = request.IsActive,
          CreatedBy = request.CreatedBy
        };
    }

    public DbPositionUser Map(EditPositionUserRequest request)
    {
      return request is null || !request.PositionId.HasValue
        ? null
        : new DbPositionUser
        {
          Id = Guid.NewGuid(),
          UserId = request.UserId,
          PositionId = request.PositionId.Value,
          IsActive = true,
          CreatedBy = _httpContextAccessor.HttpContext.GetUserId()
        };
    }
  }
}
