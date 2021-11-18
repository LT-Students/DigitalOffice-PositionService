using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;

namespace LT.DigitalOffice.PositionService.Validation.PositionUser.Interfaces
{
  [AutoInject]
  public interface ICreatePositionUserRequestValidator : IValidator<CreatePositionUserRequest>
  {
  }
}
