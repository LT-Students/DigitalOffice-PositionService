using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IGlobalCacheRepository _globalCache;

    public DisactivateUserConsumer(
      IPositionUserRepository positionUserRepository,
      IGlobalCacheRepository globalCache)
    {
      _positionUserRepository = positionUserRepository;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      Guid? positionId = await _positionUserRepository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy);

      if (positionId.HasValue)
      {
        await _globalCache.RemoveAsync(positionId.Value);
      }  
    }
  }
}
