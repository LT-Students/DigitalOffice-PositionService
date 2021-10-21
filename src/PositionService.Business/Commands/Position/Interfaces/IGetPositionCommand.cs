using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Models.Dto.Models;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces
{
  [AutoInject]
  public interface IGetPositionCommand
  {
    Task<OperationResultResponse<PositionInfo>> ExecuteAsync(Guid positionId);
  }
}
