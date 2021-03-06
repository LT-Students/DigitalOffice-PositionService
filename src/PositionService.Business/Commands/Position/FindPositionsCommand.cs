using System.Linq;
using System.Net;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Responses;
using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
using System.Collections.Generic;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position
{
  /// <inheritdoc cref="IFindPositionsCommand"/>
  public class FindPositionsCommand : IFindPositionsCommand
  {
    private readonly IBaseFindFilterValidator _baseFindFilterValidator;
    private readonly IPositionRepository _repository;
    private readonly IPositionInfoMapper _mapper;
    private readonly IResponseCreator _responseCreator;

    public FindPositionsCommand(
      IBaseFindFilterValidator baseFindFilterValidator,
      IPositionRepository repository,
      IPositionInfoMapper mapper,
      IResponseCreator responseCreator)
    {
      _baseFindFilterValidator = baseFindFilterValidator;
      _repository = repository;
      _mapper = mapper;
      _responseCreator = responseCreator;
    }

    public async Task<FindResultResponse<PositionInfo>> ExecuteAsync(FindPositionsFilter filter)
    {
      if (!_baseFindFilterValidator.ValidateCustom(filter, out List<string> errors))
      {
        return _responseCreator.CreateFailureFindResponse<PositionInfo>(HttpStatusCode.BadRequest, errors);
      }

      (List<DbPosition> positions, int totalCount) = await _repository.FindAsync(filter);

      return new()
      {
        Body = positions?.Select(_mapper.Map).ToList(),
        TotalCount = totalCount
      };
    }
  }
}
