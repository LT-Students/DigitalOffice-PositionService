using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Kernel.RedisSupport.Configurations;
using LT.DigitalOffice.Kernel.RedisSupport.Constants;
using LT.DigitalOffice.Kernel.RedisSupport.Extensions;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class GetPositionsConsumer : IConsumer<IGetPositionsRequest>
  {
    private readonly IPositionRepository _repository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IRedisHelper _redisHelper;
    private readonly ICacheNotebook _cacheNotebook;
    private readonly IPositionDataMapper _positionDataMapper;

    private async Task<List<PositionData>> GetPositionsAsync(IGetPositionsRequest request)
    {
      List<DbPosition> dbPositions = await _repository.GetAsync(request);

      return dbPositions.Select(p => _positionDataMapper.Map(p)).ToList();
    }

    public GetPositionsConsumer(
      IPositionRepository repository,
      IOptions<RedisConfig> redisConfig,
      IRedisHelper redisHelper,
      ICacheNotebook cacheNotebook,
      IPositionDataMapper positionDataMapper)
    {
      _repository = repository;
      _redisConfig = redisConfig;
      _redisHelper = redisHelper;
      _cacheNotebook = cacheNotebook;
      _positionDataMapper = positionDataMapper;
    }

    public async Task Consume(ConsumeContext<IGetPositionsRequest> context)
    {
      List<PositionData> positions = await GetPositionsAsync(context.Message);

      object response = OperationResultWrapper.CreateResponse((_) => IGetPositionsResponse.CreateObj(positions), context.Message);

      await context.RespondAsync<IOperationResult<IGetPositionsResponse>>(response);

      if (positions != null && positions.Any() && context.Message.UsersIds != null)
      {
        string key = context.Message.UsersIds.GetRedisCacheHashCode();

        await _redisHelper.CreateAsync(Cache.Positions, key, positions, TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));

        _cacheNotebook.Add(positions.Select(p => p.Id).ToList(), Cache.Positions, key);
      }
    }
  }
}
