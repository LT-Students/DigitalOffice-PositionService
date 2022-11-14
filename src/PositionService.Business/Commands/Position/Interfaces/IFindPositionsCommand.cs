using System.Threading.Tasks;
using DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;

namespace LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces
{
  [AutoInject]
  public interface IFindPositionsCommand
  {
    Task<FindResult<PositionInfo>> ExecuteAsync(FindPositionsFilter filter);
  }
}
