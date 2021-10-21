using System.Collections.Generic;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.PositionService.Validation.Position
{
  public class EditPositionRequestValidator : BaseEditRequestValidator<EditPositionRequest>, IEditPositionRequestValidator
  {
    private void HandleInternalPropertyValidation(Operation<EditPositionRequest> requestedOperation, CustomContext context)
    {
      RequestedOperation = requestedOperation;
      Context = context;

      #region Paths

      AddСorrectPaths(
        new List<string>
        {
          nameof(EditPositionRequest.Name),
          nameof(EditPositionRequest.Description),
          nameof(EditPositionRequest.IsActive)
        });

      AddСorrectOperations(nameof(EditPositionRequest.Name), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditPositionRequest.Description), new() { OperationType.Replace });
      AddСorrectOperations(nameof(EditPositionRequest.IsActive), new() { OperationType.Replace });

      #endregion

      #region Name

      AddFailureForPropertyIf(
        nameof(EditPositionRequest.Name),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString()), "Name should not be empty." },
          { x => x.value.ToString().Length < 81, "Max lenght of position name is 80 symbols." },
        },
        CascadeMode.Stop);

      #endregion

      #region Description

      AddFailureForPropertyIf(
        nameof(EditPositionRequest.Description),
        x => x == OperationType.Replace,
        new()
        {
          { x => !string.IsNullOrEmpty(x.value?.ToString()), "Description should not be empty." },
          { x => x.value.ToString().Length < 351, "Max lenght of position description is 350 symbols." },
        },
        CascadeMode.Stop);

      #endregion

      #region IsActive

      AddFailureForPropertyIf(
        nameof(EditPositionRequest.IsActive),
        x => x == OperationType.Replace,
        new()
        {
          { x => bool.TryParse(x.value.ToString(), out bool _), "Incorrect format of IsActive." },
        });

      #endregion
    }

    public EditPositionRequestValidator()
    {
      RuleForEach(x => x.Operations)
        .Custom(HandleInternalPropertyValidation);
    }
  }
}
