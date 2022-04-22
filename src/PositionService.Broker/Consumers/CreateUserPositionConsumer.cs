using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class CreateUserPositionConsumer : IConsumer<ICreateUserPositionPublish>
  {
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IDbPositionUserMapper _positionUserMapper;
    private readonly IGlobalCacheRepository _globalCache;

    private async Task<bool> CreateAsync(ICreateUserPositionPublish request)
    {
      if (!await _positionRepository.DoesExistAsync(request.PositionId))
      {
        return false;
      }

      await _positionUserRepository.CreateAsync(_positionUserMapper.Map(request));

      await _globalCache.RemoveAsync(request.PositionId);

      return true;
    }

    public CreateUserPositionConsumer(
      IPositionUserRepository positionUserRepository,
      IPositionRepository positionRepository,
      IDbPositionUserMapper positionUserMapper,
      IGlobalCacheRepository globalCache)
    {
      _positionUserRepository = positionUserRepository;
      _positionRepository = positionRepository;
      _positionUserMapper = positionUserMapper;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<ICreateUserPositionPublish> context)
    {
      object response = OperationResultWrapper.CreateResponse(CreateAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
