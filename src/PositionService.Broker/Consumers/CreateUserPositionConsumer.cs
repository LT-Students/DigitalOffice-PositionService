using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class CreateUserPositionConsumer : IConsumer<ICreateUserPositionRequest>
  {
    private readonly IPositionUserRepository _userRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IDbPositionUserMapper _positionMapper;

    private async Task<bool> CreateAsync(ICreateUserPositionRequest request)
    {
      if (!await _positionRepository.DoesExistAsync(request.PositionId))
      {
        return false;
      }

      await _userRepository.CreateAsync(_positionMapper.Map(request));

      return true;
    }

    public CreateUserPositionConsumer(
      IPositionUserRepository userRepository,
      IPositionRepository positionRepository,
      IDbPositionUserMapper userMapper)
    {
      _userRepository = userRepository;
      _positionRepository = positionRepository;
      _positionMapper = userMapper;
    }

    public async Task Consume(ConsumeContext<ICreateUserPositionRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(CreateAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
