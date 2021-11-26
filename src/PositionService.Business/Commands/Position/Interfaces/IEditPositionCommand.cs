using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces
{
  [AutoInject]
  public interface IEditPositionCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(Guid positionId, JsonPatchDocument<EditPositionRequest> request);
  }
}
