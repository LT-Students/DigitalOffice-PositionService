using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Data.Provider;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.PositionService.Data
{
  public class UserRateRepository : IUserRateRepository
  {
    private readonly IDataProvider _provider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserRateRepository(
      IDataProvider provider,
      IHttpContextAccessor httpContextAccessor)
    {
      _provider = provider;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<Guid?> CreateAsync(DbUserRate dbUserRate)
    {
      if (dbUserRate is null)
      {
        return null;
      }

      _provider.UsersRates.Add(dbUserRate);
      await _provider.SaveAsync();

      return dbUserRate.Id;
    }

    public async Task<bool> DoesExistAsync(Guid userId)
    {
      return await _provider.UsersRates.AnyAsync(u => u.UserId == userId);
    }

    public async Task<bool> EditAsync(EditUserRateRequest request)
    {
      if (request is null)
      {
        return false;
      }

      var userRate = await _provider.UsersRates.FirstOrDefaultAsync(r => r.UserId == request.UserId);
      userRate.Rate = request.Rate;
      userRate.ModifiedAtUtc = DateTime.UtcNow;
      userRate.ModifiedBy = _httpContextAccessor.HttpContext.GetUserId();
      await _provider.SaveAsync();

      return true;
    }

    public async Task RemoveAsync(Guid userId, Guid removedBy)
    {
      DbUserRate user = await _provider.UsersRates.FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);

      if (user != null)
      {
        user.IsActive = false;
        user.ModifiedAtUtc = DateTime.UtcNow;
        user.ModifiedBy = removedBy;
        await _provider.SaveAsync();
      }
    }
  }
}
