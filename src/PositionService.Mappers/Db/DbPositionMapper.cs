using System;
using System.Linq;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.PositionService.Mappers.Db
{
  public class DbPositionMapper : IDbPositionMapper
  {
    private readonly IHttpContextAccessor _httpContextAccessor;

    public DbPositionMapper(IHttpContextAccessor httpContextAccessor)
    {
      _httpContextAccessor = httpContextAccessor;
    }

    public DbPosition Map(CreatePositionRequest value)
    {
      if (value == null)
      {
        return null;
      }

      return new DbPosition
      {
        Id = Guid.NewGuid(),
        Name = value.Name,
        Description = value.Description != null && value.Description.Trim().Any() ? value.Description.Trim() : null,
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _httpContextAccessor.HttpContext.GetUserId()
      };
    }
  }
}
