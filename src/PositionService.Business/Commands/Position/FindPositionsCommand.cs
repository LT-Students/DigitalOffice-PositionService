using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position
{
  /// <inheritdoc cref="IFindPositionsCommand"/>
  public class FindPositionsCommand : IFindPositionsCommand
  {
    private readonly IPositionRepository _repository;
    private readonly IPositionInfoMapper _mapper;

    public FindPositionsCommand(
      IPositionRepository repository,
      IPositionInfoMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    public async Task<FindResultResponse<PositionInfo>> ExecuteAsync(FindPositionsFilter filter)
    {
      (List<DbPosition> positions, int totalCount) = await _repository.FindAsync(filter);

      return new()
      {
        Body = positions?.Select(_mapper.Map).ToList(),
        TotalCount = totalCount
      };
    }
  }
}
