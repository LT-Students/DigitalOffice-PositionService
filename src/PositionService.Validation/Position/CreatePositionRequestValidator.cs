using FluentValidation;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;

namespace LT.DigitalOffice.PositionService.Validation.Position
{
  public class CreatePositionRequestValidator : AbstractValidator<CreatePositionRequest>, ICreatePositionRequestValidator
  {
    public CreatePositionRequestValidator(
      IPositionRepository positionRepository)
    {
      RuleFor(position => position.Name)
        .MaximumLength(80)
        .MustAsync(async (name, _) => !await positionRepository.DoesNameExistAsync(name))
        .WithMessage("Position name should be unique.");

      When(position => position.Description != null, () =>
      {
        RuleFor(position => position.Description)
          .MaximumLength(350);
      });
    }
  }
}
