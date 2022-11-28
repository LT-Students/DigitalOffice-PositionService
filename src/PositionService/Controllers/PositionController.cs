using System;
using System.Threading.Tasks;
using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.PositionService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class PositionController : ControllerBase
  {
    [HttpGet("get")]
    public async Task<OperationResultResponse<PositionInfo>> GetAsync(
      [FromServices] IGetPositionCommand command,
      [FromQuery] Guid positionId)
    {
      return await command.ExecuteAsync(positionId);
    }

    [HttpGet("find")]
    public async Task<FindResult<PositionInfo>> FindAsync(
      [FromServices] IFindPositionsCommand command,
      [FromQuery] FindPositionsFilter filter)
    {
      return await command.ExecuteAsync(filter);
    }

    [HttpPost("create")]
    public async Task<OperationResultResponse<Guid?>> CreateAsync(
      [FromServices] ICreatePositionCommand command,
      [FromBody] CreatePositionRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPatch("edit")]
    public async Task<OperationResultResponse<bool>> EditAsync(
      [FromServices] IEditPositionCommand command,
      [FromQuery] Guid positionId,
      [FromBody] JsonPatchDocument<EditPositionRequest> request)
    {
      return await command.ExecuteAsync(positionId, request);
    }
  }
}
