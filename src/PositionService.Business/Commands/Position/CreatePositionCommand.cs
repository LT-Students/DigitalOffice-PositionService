using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position
{
  public class CreatePositionCommand : ICreatePositionCommand
  {
    private readonly ICreatePositionRequestValidator _validator;
    private readonly IPositionRepository _repository;
    private readonly IDbPositionMapper _mapper;
    private readonly IAccessValidator _accessValidator;

    public CreatePositionCommand(
      ICreatePositionRequestValidator validator,
      IPositionRepository repository,
      IDbPositionMapper mapper,
      IAccessValidator accessValidator)
    {
      _validator = validator;
      _repository = repository;
      _mapper = mapper;
      _accessValidator = accessValidator;
    }

    public async Task<OperationResultResponse<Guid?>> ExecuteAsync(CreatePositionRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemovePositions))
      {
        return ResponseCreatorStatic.CreateResponse<Guid?>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return ResponseCreatorStatic.CreateResponse<Guid?>(
          HttpStatusCode.BadRequest,
          errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      DbPosition dbPosition = _mapper.Map(request);

      await _repository.CreateAsync(dbPosition);

      return new OperationResultResponse<Guid?>(body: dbPosition.Id);
    }
  }
}
