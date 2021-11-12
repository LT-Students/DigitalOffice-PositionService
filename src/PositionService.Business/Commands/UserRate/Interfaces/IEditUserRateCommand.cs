using System;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate;

namespace LT.DigitalOffice.PositionService.Business.Commands.UserRate.Interfaces
{
  [AutoInject]
  public interface IEditUserRateCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(EditUserRateRequest request);
  }
}
