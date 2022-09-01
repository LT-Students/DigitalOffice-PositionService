using System;
using System.Threading.Tasks;
using DigitalOffice.Models.Broker.Publishing;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class ActivateUserPositionConsumer : IConsumer<IActivateUserPublish>
  {
    private readonly IPositionUserRepository _repository;
    private readonly ILogger<ActivateUserPositionConsumer> _logger;

    public ActivateUserPositionConsumer(
      IPositionUserRepository repository,
      ILogger<ActivateUserPositionConsumer> logger)
    {
      _repository = repository;
      _logger = logger;
    }

    public async Task Consume(ConsumeContext<IActivateUserPublish> context)
    {
      Guid? positionId = await _repository.ActivateAsync(context.Message);

      if (positionId.HasValue)
      {
        _logger.LogInformation("UserId '{UserId}' activated in positiontId '{PositionId}'", context.Message.UserId, positionId);
      }
    }
  }
}
