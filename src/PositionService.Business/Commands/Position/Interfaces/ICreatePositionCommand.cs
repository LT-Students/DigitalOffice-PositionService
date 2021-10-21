using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces
{
  [AutoInject]
  public interface ICreatePositionCommand
  {
    Task<OperationResultResponse<Guid?>> ExecuteAsync(CreatePositionRequest request);
  }
}
