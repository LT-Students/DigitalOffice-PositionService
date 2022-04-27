using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters
{
  public record FindPositionsFilter : BaseFindFilter
  {
    [FromQuery(Name = "isAscendingSort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name = "isActive")]
    public bool? IsActive { get; set; }
  }
}
