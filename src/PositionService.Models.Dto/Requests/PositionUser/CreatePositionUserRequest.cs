using System;

namespace LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser
{
  public record CreatePositionUserRequest
  {
    public Guid UserId { get; set; }
    public Guid PositionId { get; set; }
  }
}
