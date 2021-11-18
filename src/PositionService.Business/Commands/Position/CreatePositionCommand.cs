using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
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
    private readonly IResponseCreator _responseCreator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreatePositionCommand(
      ICreatePositionRequestValidator validator,
      IPositionRepository repository,
      IDbPositionMapper mapper,
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      IHttpContextAccessor httpContextAccessor)
    {
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _httpContextAccessor = httpContextAccessor;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreatePositionRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemovePositions))
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<Guid?>(HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
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
