using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.Models.Broker.Responses.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class FilterPositionsUsersConsumer : IConsumer<IFilterPositionsRequest>
  {
    private readonly IPositionRepository _repository;

    public async Task<List<PositionFilteredData>> GetPositionFilteredData(IFilterPositionsRequest request)
    {
      List<DbPosition> dbPosition = await _repository.GetPositionsAsync(request.PositionsIds);

      return dbPosition.Select(
        pd => new PositionFilteredData(
          pd.Id,
          pd.Name,
          pd.Users.Where(u => u.IsActive).Select(u => u.UserId).ToList())).ToList();
    }
    public FilterPositionsUsersConsumer(
      IPositionRepository repository)
    {
      _repository = repository;
    }

    public async Task Consume(ConsumeContext<IFilterPositionsRequest> context)
    {
      List<PositionFilteredData> positionFilteredData = await GetPositionFilteredData(context.Message);

      await context.RespondAsync<IOperationResult<IFilterPositionsResponse>>(
        OperationResultWrapper.CreateResponse((_) => IFilterPositionsResponse.CreateObj(positionFilteredData), context));
    }
  }
}
