using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Broker;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class CreateUserPositionConsumer : IConsumer<ICreateUserPositionRequest>
  {
    private readonly IPositionUserRepository _userRepository;
    private readonly IUserRateRepository _userRateRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IDbPositionUserMapper _positionMapper;
    private readonly IDbUserRateMapper _rateMapper;

    private async Task<bool> CreateAsync(ICreateUserPositionRequest request)
    {
      if (!await _positionRepository.DoesExistAsync(request.PositionId))
      {
        return false;
      }

      await Task.WhenAll(
        _userRepository.CreateAsync(_positionMapper.Map(request)),
        _userRateRepository.CreateAsync(_rateMapper.Map(request)));

      return true;
    }

    public CreateUserPositionConsumer(
      IPositionUserRepository userRepository,
      IUserRateRepository userRateRepository,
      IPositionRepository positionRepository,
      IDbPositionUserMapper userMapper,
      IDbUserRateMapper rateMapper)
    {
      _userRepository = userRepository;
      _userRateRepository = userRateRepository;
      _positionRepository = positionRepository;
      _positionMapper = userMapper;
      _rateMapper = rateMapper;
    }

    public async Task Consume(ConsumeContext<ICreateUserPositionRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(CreateAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
