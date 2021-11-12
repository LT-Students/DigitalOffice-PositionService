using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.PositionUser.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Validation.PositionUser.Interfaces;

namespace LT.DigitalOffice.PositionService.Business.Commands.PositionUser
{
  public class CreatePositionUserCommand : ICreatePositionUserCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IResponseCreator _responseCreator;
    private readonly ICreatePositionUserRequestValidator _validator;
    private readonly IDbPositionUserMapper _mapper;
    private readonly IPositionUserRepository _repository;
    private readonly ICacheNotebook _cacheNotebook;

    private async Task ClearCache(Guid userId, Guid newPositionId)
    {
      Guid positionId = (await _repository.GetAsync(userId)).PositionId;

      await Task.WhenAll(
        _cacheNotebook.RemoveAsync(positionId),
        _cacheNotebook.RemoveAsync(newPositionId));
    }

    public CreatePositionUserCommand(
      IAccessValidator accessValidator,
      IResponseCreator responseCreator,
      ICreatePositionUserRequestValidator validator,
      IDbPositionUserMapper mapper,
      IPositionUserRepository repository,
      ICacheNotebook cacheNotebook)
    {
      _accessValidator = accessValidator;
      _responseCreator = responseCreator;
      _validator = validator;
      _mapper = mapper;
      _repository = repository;
      _cacheNotebook = cacheNotebook;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(CreatePositionUserRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemovePositions))
      {
        return _responseCreator.CreateFailureResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return _responseCreator.CreateFailureResponse<bool>(
          HttpStatusCode.BadRequest,
          validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      Guid? result = await _repository.CreateAsync(_mapper.Map(request));

      if (result.HasValue)
      {
        await ClearCache(request.UserId, request.PositionId);
      }

      return new OperationResultResponse<bool>
      {
        Status = result.HasValue ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result.HasValue
      };
    }
  }
}
