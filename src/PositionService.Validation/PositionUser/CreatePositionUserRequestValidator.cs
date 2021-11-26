using FluentValidation;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Validation.PositionUser.Interfaces;

namespace LT.DigitalOffice.PositionService.Validation.PositionUser
{
  public class CreatePositionUserRequestValidator : AbstractValidator<CreatePositionUserRequest>, ICreatePositionUserRequestValidator
  {
    public CreatePositionUserRequestValidator(
      IPositionUserRepository positionUserRepository,
      IPositionRepository positionRepository)
    {
      RuleFor(x => x)
        .MustAsync(async (request, _) =>
        {
          var position = await positionUserRepository.GetAsync(request.UserId);
          return position is not null && position.IsActive && position.PositionId != request.PositionId;
        })
        .WithMessage("Incorrect position for this user.");

      RuleFor(x => x.PositionId)
        .MustAsync(async (id, _) => await positionRepository.DoesExistAsync(id))
        .WithMessage("Position must exist.");
    }
  }
}
