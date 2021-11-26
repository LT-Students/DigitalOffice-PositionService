using System;

namespace LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate
{
  public record EditUserRateRequest
  {
    public Guid UserId { get; set; }
    public double Rate { get; set; }
  }
}
