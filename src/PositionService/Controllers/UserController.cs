using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.PositionUser.Interfaces;
using LT.DigitalOffice.PositionService.Business.Commands.UserRate.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.UserRate;
using Microsoft.AspNetCore.Mvc;

namespace LT.DigitalOffice.PositionService.Controllers
{
  [Route("[controller]")]
  [ApiController]
  public class UserController : ControllerBase
  {
    [HttpPost("CreatePosition")]
    public async Task<OperationResultResponse<bool>> CreatePositionAsync(
      [FromServices] ICreatePositionUserCommand command,
      [FromBody] CreatePositionUserRequest request)
    {
      return await command.ExecuteAsync(request);
    }

    [HttpPut("EditRate")]
    public async Task<OperationResultResponse<bool>> EditRateAsync(
      [FromServices] IEditUserRateCommand command,
      [FromBody] EditUserRateRequest request)
    {
      return await command.ExecuteAsync(request);
    }
  }
}
