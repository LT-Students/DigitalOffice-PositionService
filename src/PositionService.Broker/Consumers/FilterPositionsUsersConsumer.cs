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
using LT.DigitalOffice.PositionService.Models.Db;
using MassTransit;
using Microsoft.Extensions.Options;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class FilterPositionsUsersConsumer : IConsumer<IFilterPositionsRequest>
  {
    private readonly IPositionRepository _repository;
    private readonly IOptions<RedisConfig> _redisConfig;
    private readonly IGlobalCacheRepository _globalCache;

    public async Task<List<PositionFilteredData>> GetPositionFilteredData(IFilterPositionsRequest request)
    {
      List<DbPosition> dbPosition = await _repository.GetAsync(request.PositionsIds);

      return dbPosition.Select(
        pd => new PositionFilteredData(
          pd.Id,
          pd.Name,
          pd.Users.Select(u => u.UserId).ToList()))
        .ToList();
    }

    public FilterPositionsUsersConsumer(
      IPositionRepository repository,
      IOptions<RedisConfig> redisConfig,
      IGlobalCacheRepository globalCache)
    {
      _repository = repository;
      _redisConfig = redisConfig;
      _globalCache = globalCache;
    }

    public async Task Consume(ConsumeContext<IFilterPositionsRequest> context)
    {
      List<PositionFilteredData> positionFilteredData = await GetPositionFilteredData(context.Message);

      await context.RespondAsync<IOperationResult<IFilterPositionsResponse>>(
        OperationResultWrapper.CreateResponse((_) => IFilterPositionsResponse.CreateObj(positionFilteredData), context));

      if (positionFilteredData is not null)
      {
        List<Guid> elementsIds = new();

        positionFilteredData.ForEach(p =>
        {
          elementsIds.Add(p.Id);

          if (p.UsersIds is not null)
          {
            elementsIds.AddRange(p.UsersIds);
          }
        });

        await _globalCache.CreateAsync(
          Cache.Positions,
          context.Message.PositionsIds.GetRedisCacheKey(nameof(IFilterPositionsRequest), context.Message.GetBasicProperties()),
          positionFilteredData,
          elementsIds,
          TimeSpan.FromMinutes(_redisConfig.Value.CacheLiveInMinutes));
      }
    }
  }
}
