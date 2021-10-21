using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position
{
  public class CreatePositionCommand : ICreatePositionCommand
  {
    private readonly ICreatePositionRequestValidator _validator;
    private readonly IPositionRepository _repository;
    private readonly IDbPositionMapper _mapper;
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreater _responseCreater;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePositionCommand(
      ICreatePositionRequestValidator validator,
      IPositionRepository repository,
      IDbPositionMapper mapper,
      IAccessValidator accessValidator,
      IResponseCreater responseCreater,
      IHttpContextAccessor httpContextAccessor)
    {
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _responseCreater = responseCreater;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreatePositionRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemovePositions))
      {
        return _responseCreater.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      if (!_validator.ValidateCustom(request, out List<string> errors))
      {
        return _responseCreater.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest, errors);
      }

      if (await _repository.DoesNameExistAsync(request.Name))
      {
        errors.Add("Position name should be unique.");

        return _responseCreater.CreateFailureResponse<Guid?>(HttpStatusCode.Conflict, errors);
      }

      _httpContextAccessor.HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;

      return new()
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = await _repository.CreateAsync(_mapper.Map(request))
      };
    }
  }
}
