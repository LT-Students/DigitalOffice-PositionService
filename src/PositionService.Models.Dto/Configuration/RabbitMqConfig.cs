using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;

namespace LT.DigitalOffice.PositionService.Models.Dto.Configuration
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    public string CreateUserPositionEndpoint { get; set; }
    public string GetPositionsEndpoint { get; set; }
    public string DisactivateUserEndpoint { get; set; }
  }
}
