using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class CreateUserPositionConsumer : IConsumer<ICreateUserPositionPublish>
  {
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IDbPositionUserMapper _positionUserMapper;
    private readonly IGlobalCacheRepository _globalCache;
    private readonly IOptions<RedisConfig> _redisConfig;

    private async Task CreateAsync(ICreateUserPositionPublish request)
    {
      if (await _positionRepository.DoesExistAsync(request.PositionId))
      {
        await _positionUserRepository.CreateAsync(_positionUserMapper.Map(request));

        string key = request.UserId.GetRedisCacheHashCode();

        await _globalCache.CreateAsync(
          Cache.Positions,
          key,
          request.UserId,
          request.PositionId,
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }

    public CreateUserPositionConsumer(
      IPositionUserRepository positionUserRepository,
      IPositionRepository positionRepository,
      IDbPositionUserMapper positionUserMapper,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _positionUserRepository = positionUserRepository;
      _positionRepository = positionRepository;
      _positionUserMapper = positionUserMapper;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<ICreateUserPositionPublish> context)
    {
      await CreateAsync(context.Message);
    }
  }
}
