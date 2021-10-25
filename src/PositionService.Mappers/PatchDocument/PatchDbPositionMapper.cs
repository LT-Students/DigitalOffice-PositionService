using LT.DigitalOffice.PositionService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace LT.DigitalOffice.PositionService.Mappers.PatchDocument
{
  public class PatchDbPositionMapper : IPatchDbPositionMapper
  {
    public JsonPatchDocument<DbPosition> Map(JsonPatchDocument<EditPositionRequest> request)
    {
      if (request == null)
      {
        return null;
      }

      var result = new JsonPatchDocument<DbPosition>();

      foreach (var item in request.Operations)
      {
        result.Operations.Add(new Operation<DbPosition>(item.op, item.path, item.from, item.value));
      }

      return result;
    }
  }
}
