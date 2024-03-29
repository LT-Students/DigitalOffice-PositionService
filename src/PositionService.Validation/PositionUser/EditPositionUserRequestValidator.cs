﻿using System;
using System.Collections.Generic;
using FluentValidation;
using LT.DigitalOffice.PositionService.Broker.Requests.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Validation.PositionUser.Interfaces;

namespace LT.DigitalOffice.PositionService.Validation.PositionUser
{
  public class EditPositionUserRequestValidator : AbstractValidator<EditPositionUserRequest>, IEditPositionUserRequestValidator
  {
    public EditPositionUserRequestValidator(
      IPositionRepository positionRepository,
      IUserService userService)
    {
      RuleFor(request => request.UserId)
        .MustAsync(async (userId, _) => 
          (await userService.CheckUsersExistenceAsync(new List<Guid>() { userId }))?.Count == 1)
        .WithMessage("This user's position cannot be changed.");

      When(request =>
        request.PositionId.HasValue,
        () =>
          RuleFor(request => request.PositionId)
            .MustAsync(async (id, _) => await positionRepository.DoesExistAsync(id.Value))
            .WithMessage("Position must exist."));
    }
  }
}
