using System;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class ActivateUserPositionConsumer : IConsumer<IActivateUserPublish>
  {
    private readonly IPositionUserRepository _repository;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly ILogger<ActivateUserPositionConsumer> _logger;

    public ActivateUserPositionConsumer(
      IPositionUserRepository repository,
      IGlobalCacheRepository globalCache,
      ILogger<ActivateUserPositionConsumer> logger)
    {
      _repository = repository;
      _globalCache = globalCache;
      _logger = logger;
    }

    public async Task Consume(ConsumeContext<IActivateUserPublish> context)
    {
      Guid? positionId = await _repository.ActivateAsync(context.Message);

      if (positionId.HasValue)
      {
        await _globalCache.RemoveAsync(positionId.Value);

        _logger.LogInformation("UserId '{UserId}' activated in positiontId '{PositionId}'", context.Message.UserId, positionId);
      }
    }
  }
}
