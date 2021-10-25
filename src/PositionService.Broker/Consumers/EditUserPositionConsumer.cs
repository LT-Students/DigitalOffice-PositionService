using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.Kernel.Broker;
using MassTransit;
using LT.DigitalOffice.Models.Broker.Requests.Position;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class EditUserPositionConsumer : IConsumer<IEditUserPositionRequest>
  {
    private readonly IPositionRepository _positionRepository;
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IDbPositionUserMapper _positionUserMapper;

    private async Task<bool> ChangePositionAsync(IEditUserPositionRequest request)
    {
      if (!await _positionRepository.DoesExistAsync(request.PositionId))
      {
        return false;
      }

      await _positionUserRepository.RemoveAsync(request.UserId, request.ModifiedBy);
      return await _positionUserRepository.CreateAsync(_positionUserMapper.Map(request)) != null;
    }

    public EditUserPositionConsumer(
      IPositionRepository positionRepository,
      IPositionUserRepository positionUserRepository,
      IDbPositionUserMapper positionUserMapper)
    {
      _positionRepository = positionRepository;
      _positionUserRepository = positionUserRepository;
      _positionUserMapper = positionUserMapper;
    }

    public async Task Consume(ConsumeContext<IEditUserPositionRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(ChangePositionAsync, context.Message);

      await context.RespondAsync<IOperationResult<(bool department, bool position, bool office)>>(response);
    }
  }
}
