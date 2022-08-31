using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Validators;
using LT.DigitalOffice.Kernel.Validators;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;
using LT.DigitalOffice.PositionService.Validation.Position.Resources;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.PositionService.Validation.Position
{
  public class EditPositionRequestValidator : BaseEditRequestValidator<EditPositionRequest>, IEditPositionRequestValidator
  {
    private readonly IPositionRepository _positionRepository;

    private async Task HandleInternalPropertyValidationAsync(Operation<EditPositionRequest> requestedOperation, CustomContext context)
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
          { x => !string.IsNullOrEmpty(x.value?.ToString()), string.Join(nameof(EditPositionRequest.Name), PositionRequestValidationResource.NotNullOrEmpy) },
          { x => x.value.ToString().Length < 81, PositionRequestValidationResource.NameLong }
        },
        CascadeMode.Stop);

      await AddFailureForPropertyIfAsync(
        nameof(EditPositionRequest.Name),
        x => x == OperationType.Replace,
        new()
        {
          { async x => !await _positionRepository.DoesNameExistAsync(x.value?.ToString()), PositionRequestValidationResource.NameExists }
        });

      #endregion

      #region Description

      AddFailureForPropertyIf(
        nameof(EditPositionRequest.Description),
        x => x == OperationType.Replace,
        new()
        {
          { x => x.value?.ToString()?.Length < 351, PositionRequestValidationResource.DescriptionLong }
        });

      #endregion

      #region IsActive

      AddFailureForPropertyIf(
        nameof(EditPositionRequest.IsActive),
        x => x == OperationType.Replace,
        new()
        {
          { x => bool.TryParse(x.value.ToString(), out bool _), string.Join(PositionRequestValidationResource.NameExists, nameof(EditPositionRequest.IsActive))}
        });

      #endregion
    }

    public EditPositionRequestValidator(IPositionRepository positionRepository)
    {
      _positionRepository = positionRepository;

      Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");

      RuleForEach(x => x.Operations)
        .CustomAsync(async (operation, context, _) => await HandleInternalPropertyValidationAsync(operation, context));
    }
  }
}
