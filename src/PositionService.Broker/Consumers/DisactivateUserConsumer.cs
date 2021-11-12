using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.Models.Broker.Common;
using MassTransit;

namespace LT.DigitalOffice.PositionService.Broker.Consumers
{
  public class DisactivateUserConsumer : IConsumer<IDisactivateUserRequest>
  {
    private readonly IPositionUserRepository _positionUserRepository;
    private readonly IUserRateRepository _userRateRepository;

    public DisactivateUserConsumer(
      IPositionUserRepository positionUserRepository,
      IUserRateRepository userRateRepository)
    {
      _positionUserRepository = positionUserRepository;
      _userRateRepository = userRateRepository;
    }

    public async Task Consume(ConsumeContext<IDisactivateUserRequest> context)
    {
      await Task.WhenAll(
        _positionUserRepository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy),
        _userRateRepository.RemoveAsync(context.Message.UserId, context.Message.ModifiedBy));
    }
  }
}
