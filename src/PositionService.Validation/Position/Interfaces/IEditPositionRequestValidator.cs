using FluentValidation;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.Kernel.Attributes;
using Microsoft.AspNetCore.JsonPatch;
using System;

namespace LT.DigitalOffice.PositionService.Validation.Position.Interfaces
{
    [AutoInject]
    public interface IEditPositionRequestValidator : IValidator<(Guid, JsonPatchDocument<EditPositionRequest>)>
    {
    }
}
