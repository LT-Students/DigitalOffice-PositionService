using System;

namespace LT.DigitalOffice.PositionService.Models.Dto.Models
{
  public class UserInfo
  {
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public ImageInfo Image { get; set; }
  }
}
