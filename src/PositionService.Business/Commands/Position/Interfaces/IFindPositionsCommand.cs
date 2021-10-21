using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces
{
  [AutoInject]
  public interface IFindPositionsCommand
  {
    Task<FindResultResponse<PositionInfo>> ExecuteAsync(FindPositionsFilter filter);
  }
}
