using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class CreateUserPositionConsumer : IConsumer<ICreateUserPositionPublish>
  {
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IPositionRepository _positionRepository;
    private readonly IDbPositionUserMapper _positionUserMapper;

    public CreateUserPositionConsumer(
      IPositionUserRepository positionUserRepository,
      IPositionRepository positionRepository,
      IDbPositionUserMapper positionUserMapper)
    {
      _positionUserRepository = positionUserRepository;
      _positionRepository = positionRepository;
      _positionUserMapper = positionUserMapper;
    }

    public async Task Consume(ConsumeContext<ICreateUserPositionPublish> context)
    {
      if (await _positionRepository.DoesExistAsync(context.Message.PositionId))
      {
        await _positionUserRepository.CreateAsync(_positionUserMapper.Map(context.Message));
      }
    }
  }
}
