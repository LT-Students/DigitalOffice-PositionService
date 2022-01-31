using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;

namespace LT.DigitalOffice.PositionService.Business.Commands.PositionUser.Interfaces
{
  [AutoInject]
  public interface IEditPositionUserCommand
  {
    Task<OperationResultResponse<bool>> ExecuteAsync(EditPositionUserRequest request);
  }
}
