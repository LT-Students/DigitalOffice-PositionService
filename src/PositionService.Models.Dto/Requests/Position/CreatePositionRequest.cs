using System.ComponentModel.DataAnnotations;

namespace LT.DigitalOffice.PositionService.Models.Dto.Requests.Position
{
  public record CreatePositionRequest
  {
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
  }
}
