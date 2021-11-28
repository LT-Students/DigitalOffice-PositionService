using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IPositionUserRepository _positionUserRepository;

    public DisactivateUserConsumer(
      IPositionUserRepository positionUserRepository)
    {
      _positionUserRepository = positionUserRepository;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      await Task.WhenAll(
        _positionUserRepository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy));
    }
  }
}
