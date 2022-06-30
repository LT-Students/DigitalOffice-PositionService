using System.Net;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Responses;
using System.Threading.Tasks;
using System;
using LT.DigitalOffice.Kernel.Helpers;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position
{
  public class GetPositionCommand : IGetPositionCommand
  {
    private readonly IPositionRepository _repository;
    private readonly IPositionInfoMapper _mapper;

    public GetPositionCommand(
      IPositionRepository repository,
      IPositionInfoMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    public async Task<OperationResultResponse<PositionInfo>> ExecuteAsync(Guid positionId)
    {
      DbPosition position = await _repository.GetAsync(positionId);

      return position is null
        ? ResponseCreatorStatic.CreateResponse<PositionInfo>(HttpStatusCode.NotFound)
        : new()
          {
            Body = _mapper.Map(position)
          };
    }
  }
}
