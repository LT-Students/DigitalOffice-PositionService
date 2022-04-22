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
      if (request is null)
      {
        return null;
      }

      return new DbPositionUser
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        PositionId = request.PositionId,
        IsActive = true,
        CreatedBy = request.CreatedBy,
        CreatedAtUtc = DateTime.UtcNow
      };
    }

    public DbPositionUser Map(EditPositionUserRequest request)
    {
      if (request is null)
      {
        return null;
      }

      return new DbPositionUser
      {
        Id = Guid.NewGuid(),
        UserId = request.UserId,
        PositionId = request.PositionId.Value,
        IsActive = true,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId(),
        CreatedAtUtc = DateTime.UtcNow
      };
    }
  }
}
