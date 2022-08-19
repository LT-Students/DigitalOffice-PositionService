using LT.DigitalOffice.Kernel.BrokerSupport.Attributes;
using LT.DigitalOffice.Kernel.BrokerSupport.Configurations;
using LT.DigitalOffice.Models.Broker.Common;

namespace LT.DigitalOffice.PositionService.Models.Dto.Configuration
{
  public class RabbitMqConfig : BaseRabbitMqConfig
  {
    public string CreateUserPositionEndpoint { get; set; }
    public string GetPositionsEndpoint { get; set; }
    public string DisactivatePositionUserEndpoint { get; set; }
    public string FilterPositionsEndpoint { get; set; }

    [AutoInjectRequest(typeof(ICheckUsersExistence))]
    public string CheckUsersExistenceEndpoint { get; set; }
  }
}
