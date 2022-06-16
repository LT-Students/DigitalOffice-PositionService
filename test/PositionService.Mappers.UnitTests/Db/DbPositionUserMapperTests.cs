using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Publishing.Subscriber.Position;
using LT.DigitalOffice.PositionService.Mappers.Db;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Mappers.UnitTests.Db
{
  public class DbPositionUserMapperTests
  {
    private AutoMocker _mocker;
    private IDbPositionUserMapper _dbPositionUserMapper;
    private EditPositionUserRequest _request;
    private EditPositionUserRequest _requestWithNullPosition;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new();
      _dbPositionUserMapper = _mocker.CreateInstance<DbPositionUserMapper>();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", Guid.NewGuid());

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _request = new()
      {
        UserId = Guid.NewGuid(),
        PositionId = Guid.NewGuid()
      };

      _requestWithNullPosition = new()
      {
        UserId = Guid.NewGuid(),
        PositionId = null
      };
    }

    [Test]
    public void RequestIsNotNull()
    {
      DbPositionUser result = _dbPositionUserMapper.Map(_request);

      SerializerAssert.AreEqual(_request.UserId, result.UserId);
      SerializerAssert.AreEqual(_request.PositionId, result.PositionId);
    }

    [Test]
    public void RequestIsNull()
    {
      SerializerAssert.AreEqual(null, _dbPositionUserMapper.Map((EditPositionUserRequest)null));
      SerializerAssert.AreEqual(null, _dbPositionUserMapper.Map(_requestWithNullPosition));
    }
  }
}
