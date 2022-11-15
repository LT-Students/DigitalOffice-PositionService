using LT.DigitalOffice.Kernel.Requests;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters
{
  public record FindPositionsFilter : BaseFindFilter
  {
    [FromQuery(Name = "isAscendingSort")]
    public bool? IsAscendingSort { get; set; }

    [FromQuery(Name = "includeDeactivated")]
    public bool IncludeDeactivated { get; set; }

    [FromQuery(Name = "nameIncludeSubstring")]
    public string NameIncludeSubstring { get; set; }
  }
}
