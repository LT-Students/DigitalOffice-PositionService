using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using Microsoft.AspNetCore.JsonPatch;

namespace LT.DigitalOffice.PositionService.Mappers.PatchDocument.Interfaces
{
  [AutoInject]
  public interface IPatchDbPositionMapper
  {
    JsonPatchDocument<DbPosition> Map(JsonPatchDocument<EditPositionRequest> request);
  }
}
