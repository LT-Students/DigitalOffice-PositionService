using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position
{
  public class EditPositionCommand : IEditPositionCommand
  {
    private readonly IEditPositionRequestValidator _validator;
    private readonly IPositionRepository _repository;
    private readonly IPatchDbPositionMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly ICacheNotebook _cacheNotebook;

    public EditPositionCommand(
      IEditPositionRequestValidator validator,
      IPositionRepository repository,
      IPatchDbPositionMapper mapper,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      ICacheNotebook cacheNotebook)
    {
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _cacheNotebook = cacheNotebook;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(Guid positionId, JsonPatchDocument<EditPositionRequest> request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemovePositions))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, errors);
      }

      DbPosition position = await _repository.GetAsync(positionId);

      if (position == null)
      {
        errors.Add($"Position with id: '{position}' doesn't exist.");

        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.NotFound, errors);
      }

      foreach (var item in request.Operations)
      {
        if (item.path.EndsWith(nameof(EditPositionRequest.IsActive), StringComparison.OrdinalIgnoreCase) &&
          !bool.Parse(item.value.ToString()) &&
          await _repository.ContainsUsersAsync(positionId))
        {
          errors.Add("The position contains users. Please change the position to users");

          return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Conflict, errors);
        }
      }

      bool result = await _repository.EditAsync(position, _mapper.Map(request));

      if (result)
      {
        await _cacheNotebook.RemoveAsync(positionId);
      }

      return new()
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}
