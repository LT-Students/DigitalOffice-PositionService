using System;
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
    private readonly IPositionRepository _positionRepository;
    private readonly IDbPositionUserMapper _mapper;

    private async Task<bool> CreateAsync(ICreateUserPositionRequest request)
    {
      if (!await _positionRepository.DoesExistAsync(request.PositionId))
      {
        return false;
      }

      return (await _userRepository.CreateAsync(_mapper.Map(request))) != null;
    }

    public CreateUserPositionConsumer(
      IPositionUserRepository userRepository,
      IPositionRepository positionRepository,
      IDbPositionUserMapper mapper)
    {
      _userRepository = userRepository;
      _positionRepository = positionRepository;
      _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<ICreateUserPositionRequest> context)
    {
      object response = OperationResultWrapper.CreateResponse(CreateAsync, context.Message);

      await context.RespondAsync<IOperationResult<bool>>(response);
    }
  }
}
