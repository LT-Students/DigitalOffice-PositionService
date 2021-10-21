using FluentValidation;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;

namespace LT.DigitalOffice.PositionService.Validation.Position
{
  public class CreatePositionRequestValidator : AbstractValidator<CreatePositionRequest>, ICreatePositionRequestValidator
  {
    public CreatePositionRequestValidator()
    {
      RuleFor(position => position.Name)
        .NotEmpty()
        .MaximumLength(80);

      When(position => position.Description != null, () =>
      {
        RuleFor(position => position.Description)
          .MaximumLength(350);
      });
    }
  }
}
