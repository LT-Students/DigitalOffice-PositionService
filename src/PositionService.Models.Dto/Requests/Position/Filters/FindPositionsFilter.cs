using LT.DigitalOffice.Kernel.Requests;

namespace LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters
{
  public record FindPositionsFilter : BaseFindFilter
  {
    public bool IncludeDeactivated { get; init; } = false;
  }
}
