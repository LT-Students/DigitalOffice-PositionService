using System;
using FluentValidation;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate;

namespace LT.DigitalOffice.PositionService.Validation.UserRate.Interfaces
{
  [AutoInject]
  public interface IEditUserRateRequestValidator : IValidator<EditUserRateRequest>
  {
  }
}
