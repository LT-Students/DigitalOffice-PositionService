using FluentValidation;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate;
using LT.DigitalOffice.PositionService.Validation.UserRate.Interfaces;

namespace LT.DigitalOffice.PositionService.Validation.UserRate
{
  public class EditUserRateRequestValidator : AbstractValidator<EditUserRateRequest>, IEditUserRateRequestValidator
  {
    public EditUserRateRequestValidator(IUserRateRepository repository)
    {
      RuleFor(x => x.Rate)
        .LessThanOrEqualTo(1)
        .GreaterThan(0);

      RuleFor(x => x.UserId)
        .MustAsync(async (id, _) => await repository.DoesExistAsync(id))
        .WithMessage("User must exist.");
    }
  }
}
