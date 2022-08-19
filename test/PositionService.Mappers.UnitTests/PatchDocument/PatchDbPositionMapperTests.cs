using System;
using System.Collections.Generic;
using LT.DigitalOffice.PositionService.Mappers.PatchDocument;
using LT.DigitalOffice.PositionService.Mappers.PatchDocument.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Mappers.UnitTests.PatchDocument
{
  public class PatchDbPositionMapperTests
  {
    private IPatchDbPositionMapper _mapper;

    private JsonPatchDocument<EditPositionRequest> _request;
    private JsonPatchDocument<DbPosition> _result;

    private Guid? _imageId;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _imageId = Guid.NewGuid();

      _mapper = new PatchDbPositionMapper();

      _request = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
        {
          new Operation<EditPositionRequest>(
            "replace",
            $"/{nameof(EditPositionRequest.Name)}",
            "",
            "Name"),
          new Operation<EditPositionRequest>(
            "replace",
            $"/{nameof(EditPositionRequest.Description)}",
            "",
            "Description"),
          new Operation<EditPositionRequest>(
            "replace",
            $"/{nameof(EditPositionRequest.IsActive)}",
            "",
            true)
        }, new CamelCasePropertyNamesContractResolver());

      _result = new JsonPatchDocument<DbPosition>(new List<Operation<DbPosition>>
            {
                new Operation<DbPosition>(
                    "replace",
                    $"/{nameof(DbPosition.Name)}",
                    "",
                    "Name"),
                new Operation<DbPosition>(
                    "replace",
                    $"/{nameof(DbPosition.Description)}",
                    "",
                    "Description"),
                new Operation<DbPosition>(
                    "replace",
                    $"/{nameof(DbPosition.IsActive)}",
                    "",
                    true)
            }, new CamelCasePropertyNamesContractResolver());
    }

    [Test]
    public void ShouldReturnCorrectResponse()
    {
      SerializerAssert.AreEqual(_result, _mapper.Map(_request));
    }

    [Test]
    public void ShouldThrowExceptionWhenRequestNull()
    {
      Assert.Null(_mapper.Map(null));
    }
  }
}
