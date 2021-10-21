using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Models.Broker.Models.Company;
using MassTransit;
using Microsoft.Extensions.Options;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.UserService.Models.Dto.Configurations;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Models.Broker.Responses.Position;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class GetPositionsConsumer : IConsumer<IGetPositionsRequest>
  {
    private readonly IPositionRepository _repository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IRedisHelper _redisHelper;
    private readonly ICacheNotebook _cacheNotebook;

    private async Task<List<PositionData>> GetPositionAsync(IGetPositionsRequest request)
    {
      List<DbPosition> positions = await _repository.GetAsync(request.PositionsIds, includeUsers: true);

      return positions.Select(
          p => new PositionData(p.Id, p.Name,
            p.Users.Select(u => u.Id).ToList()))
        .ToList();
    }

    public GetPositionsConsumer(
      IPositionRepository repository,
      IOptions<RedisConfig> redisConfig,
      IRedisHelper redisHelper,
      ICacheNotebook cacheNotebook)
    {
      _repository = repository;
      _redisConfig = redisConfig;
      _redisHelper = redisHelper;
      _cacheNotebook = cacheNotebook;
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
