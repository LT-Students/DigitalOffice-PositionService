using System;
using System.Collections.Generic;
using LT.DigitalOffice.PositionService.Mappers.Db;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Mappers.UnitTests.Db
{
  public class DbPositionMapperTests
  {
    private AutoMocker _mocker;
    private IDbPositionMapper _dbPositionMapper;
    private CreatePositionRequest _request;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new();
      _dbPositionMapper = _mocker.CreateInstance<DbPositionMapper>();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", Guid.NewGuid());

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _request = new()
      {
        Name = "Name",
        Description = "Description"
      };
    }

    [Test]
    public void RequestIsNotNull()
    {
      SerializerAssert.AreEqual(_request.Name, _dbPositionMapper.Map(_request).Name);
      SerializerAssert.AreEqual(_request.Description, _dbPositionMapper.Map(_request).Description);
      SerializerAssert.AreEqual(true, _dbPositionMapper.Map(_request).IsActive);
    }

    [Test]
    public void RequestIsNull()
    {
      SerializerAssert.AreEqual(null, _dbPositionMapper.Map(null));
    }
  }
}
