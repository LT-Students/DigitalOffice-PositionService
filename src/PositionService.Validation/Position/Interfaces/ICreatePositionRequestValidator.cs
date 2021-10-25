using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;

namespace LT.DigitalOffice.PositionService.Validation.Position.Interfaces
{
  [AutoInject]
    public interface ICreatePositionRequestValidator : IValidator<CreatePositionRequest>
    {
    }
}
