using System.Globalization;
using System.Threading;
using FluentValidation;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;
using LT.DigitalOffice.PositionService.Validation.Position.Resources;

namespace LT.DigitalOffice.PositionService.Validation.Position
{
  public class CreatePositionRequestValidator : AbstractValidator<CreatePositionRequest>, ICreatePositionRequestValidator
  {
    public CreatePositionRequestValidator(
      IPositionRepository positionRepository)
    {
      Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

      RuleFor(position => position.Name)
        .MaximumLength(80)
        .WithMessage(PositionRequestValidationResource.NameLong)
        .MustAsync(async (name, _) => !await positionRepository.DoesNameExistAsync(name))
        .WithMessage(PositionRequestValidationResource.NameExists);

      When(position => position.Description != null, () =>
      {
        RuleFor(position => position.Description)
          .MaximumLength(350)
          .WithMessage(PositionRequestValidationResource.DescriptionLong);
      });
    }
  }
}
