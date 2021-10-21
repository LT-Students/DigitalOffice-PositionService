using System.Net;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using System;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position
{
  public class GetPositionCommand : IGetPositionCommand
  {
    private readonly IPositionRepository _repository;
    private readonly IPositionInfoMapper _mapper;
    private readonly IResponseCreater _responseCreater;

    public GetPositionCommand(
      IPositionRepository repository,
      IPositionInfoMapper mapper,
      IResponseCreater responseCreater)
    {
      _repository = repository;
      _mapper = mapper;
      _responseCreater = responseCreater;
    }

    public async Task<OperationResultResponse<PositionInfo>> ExecuteAsync(Guid positionId)
    {
      DbPosition position = await _repository.GetAsync(positionId);

      if (position == null)
      {
        return _responseCreater.CreateFailureResponse<PositionInfo>(HttpStatusCode.NotFound);
      }

      return new()
      {
        Status = OperationResultStatusType.FullSuccess,
        Body = _mapper.Map(position)
      };
    }
  }
}
