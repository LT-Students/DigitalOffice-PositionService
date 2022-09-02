using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class CreateUserPositionConsumer : IConsumer<ICreateUserPositionPublish>
  {
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IDbPositionUserMapper _positionUserMapper;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly ILogger<CreateUserPositionConsumer> _logger;

    public CreateUserPositionConsumer(
      IPositionUserRepository positionUserRepository,
      IPositionRepository positionRepository,
      IDbPositionUserMapper positionUserMapper,
      IGlobalCacheRepository globalCache,
      ILogger<CreateUserPositionConsumer> logger)
    {
      _positionUserRepository = positionUserRepository;
      _positionRepository = positionRepository;
      _positionUserMapper = positionUserMapper;
      _globalCache = globalCache;
      _logger = logger;
    }

    public async Task Consume(ConsumeContext<ICreateUserPositionPublish> context)
    {
      if (await _positionRepository.DoesExistAsync(context.Message.PositionId))
      {
        await _positionUserRepository.CreateAsync(_positionUserMapper.Map(context.Message));

        if (context.Message.IsActive)
        {
          await _globalCache.RemoveAsync(context.Message.PositionId);
        }
      }
      else
      {
        _logger.LogError(
          "Cannot create userId '{UserId}' for departmentId '{DepartmentId}'",
          context.Message.UserId,
          context.Message.PositionId);
      }
    }
  }
}
