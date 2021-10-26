using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using MassTransit;
using Microsoft.Extensions.Options;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Mappers.Data.Interfaces;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class GetPositionsConsumer : IConsumer<IGetPositionsRequest>
  {
    private readonly IPositionRepository _repository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IRedisHelper _redisHelper;
    private readonly ICacheNotebook _cacheNotebook;
    private readonly IPositionDataMapper _positionDataMapper;

    private async Task<List<PositionData>> GetPositionAsync(IGetPositionsRequest request)
    {
      List<DbPosition> positions = await _repository.GetAsync(request);

      return positions.Select(_positionDataMapper.Map).ToList();
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
      List<PositionData> positions = await GetPositionAsync(context.Message);

      object response = OperationResultWrapper.CreateResponse((_) => IGetPositionsResponse.CreateObj(positions), context.Message);

      await context.RespondAsync<IOperationResult<IGetPositionsResponse>>(response);

      if (positions != null)
      {
        List<Guid> positionsIds = positions.Select(p => p.Id).ToList();
        string key = positionsIds.GetRedisCacheHashCode();

        await _redisHelper.CreateAsync(Cache.Positions, key, positions, TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));

        _cacheNotebook.Add(positionsIds, Cache.Positions, key);
      }
    }
  }
}
