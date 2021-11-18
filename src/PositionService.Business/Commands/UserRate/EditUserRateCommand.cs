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
using LT.DigitalOffice.PositionService.Business.Commands.UserRate.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate;
using LT.DigitalOffice.PositionService.Validation.UserRate.Interfaces;

namespace LT.DigitalOffice.PositionService.Business.Commands.UserRate
{
  public class EditUserRateCommand : IEditUserRateCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IUserRateRepository _repository;
    private readonly IPositionUserRepository _userRepository;
    private readonly IEditUserRateRequestValidator _validator;
    private readonly IResponseCreator _responseCreator;
    private readonly ICacheNotebook _cacheNotebook;

    private async Task ClearCache(Guid userId)
    {
      Guid positionId = (await _userRepository.GetAsync(userId)).PositionId;

      await _cacheNotebook.RemoveAsync(positionId);
    }

    public EditUserRateCommand(
      IAccessValidator accessValidator,
      IUserRateRepository repository,
      IPositionUserRepository userRepository,
      IEditUserRateRequestValidator validator,
      IResponseCreator responseCreator,
      ICacheNotebook cacheNotebook)
    {
      _accessValidator = accessValidator;
      _repository = repository;
      _userRepository = userRepository;
      _validator = validator;
      _responseCreator = responseCreator;
      _cacheNotebook = cacheNotebook;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditUserRateRequest request)
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

      bool result = await _repository.EditAsync(request);

      if (result)
      {
        await ClearCache(request.UserId);
      }

      return new OperationResultResponse<bool>
      {
        Status = result ? OperationResultStatusType.FullSuccess : OperationResultStatusType.Failed,
        Body = result
      };
    }
  }
}
