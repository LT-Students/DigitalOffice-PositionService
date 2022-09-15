using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class DisactivateUserPositionConsumer : IConsumer<IDisactivateUserPublish>
  {
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IGlobalCacheRepository _globalCache;

    public DisactivateUserPositionConsumer(
      IPositionUserRepository positionUserRepository,
      IGlobalCacheRepository globalCache)
    {
      _positionUserRepository = positionUserRepository;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserPublish> context)
    {
      Guid? positionId = await _positionUserRepository
        .RemoveAsync(context.Message.UserId, context.Message.ModifiedBy);

      if (positionId.HasValue)
      {
        await _globalCache.RemoveAsync(positionId.Value);
      }
    }
  }
}
