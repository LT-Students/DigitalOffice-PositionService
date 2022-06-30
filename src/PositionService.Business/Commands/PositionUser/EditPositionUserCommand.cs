using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Extensions;
using LT.DigitalOffice.Kernel.Helpers;
using LT.DigitalOffice.Kernel.RedisSupport.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.PositionUser.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Validation.PositionUser.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LT.DigitalOffice.PositionService.Business.Commands.PositionUser
{
  public class EditPositionUserCommand : IEditPositionUserCommand
  {
    private readonly IAccessValidator _accessValidator;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IEditPositionUserRequestValidator _validator;
    private readonly IDbPositionUserMapper _mapper;
    private readonly IPositionUserRepository _repository;
    private readonly IGlobalCacheRepository _globalCache;

    public EditPositionUserCommand(
      IAccessValidator accessValidator,
      IHttpContextAccessor httpContextAccessor,
      IEditPositionUserRequestValidator validator,
      IDbPositionUserMapper mapper,
      IPositionUserRepository repository,
      IGlobalCacheRepository globalCache)
    {
      _accessValidator = accessValidator;
      _httpContextAccessor = httpContextAccessor;
      _validator = validator;
      _mapper = mapper;
      _repository = repository;
      _globalCache = globalCache;
    }

    public async Task<OperationResultResponse<bool>> ExecuteAsync(EditPositionUserRequest request)
    {
      if (!await _accessValidator.HasRightsAsync(Rights.AddEditRemovePositions))
      {
        return ResponseCreatorStatic.CreateResponse<bool>(HttpStatusCode.Forbidden);
      }

      ValidationResult validationResult = await _validator.ValidateAsync(request);

      if (!validationResult.IsValid)
      {
        return ResponseCreatorStatic.CreateResponse<bool>(
          HttpStatusCode.BadRequest,
          errors: validationResult.Errors.Select(e => e.ErrorMessage).ToList());
      }

      bool result = false;

      if (await _repository.DoesExistAsync(request.UserId))
      {
        await _globalCache.RemoveAsync(request.PositionId.GetValueOrDefault());
        await _globalCache.RemoveAsync((await _repository.GetAsync(request.UserId)).PositionId);

        result = request.PositionId.HasValue
          ? (await _repository.EditAsync(request.UserId, request.PositionId.Value)).HasValue
          : (await _repository.RemoveAsync(request.UserId, _httpContextAccessor.HttpContext.GetUserId())).HasValue;
      }
      else
      {
        if (!request.PositionId.HasValue)
        {
          return ResponseCreatorStatic.CreateResponse<bool>(HttpStatusCode.BadRequest);
        }

        result = (await _repository.CreateAsync(_mapper.Map(request))).HasValue;
      }

      return new()
      {
        Body = result
      };
    }
  }
}
