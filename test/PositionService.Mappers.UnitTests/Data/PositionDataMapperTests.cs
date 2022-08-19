using System;
using System.Collections.Generic;
using System.Linq;
using LT.DigitalOffice.Models.Broker.Models.Position;
using LT.DigitalOffice.PositionService.Mappers.Data;
using LT.DigitalOffice.PositionService.Mappers.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Moq.AutoMock;
using NUnit.Framework;

namespace PositionService.Mappers.UnitTests
{
  public class PositionDataMapperTests
  {
    private AutoMocker _mocker;
    private IPositionDataMapper _positionDataMapper;
    private DbPosition _dbPosition;
    private ICollection<DbPositionUser> _users;
    private DbPositionUser _user;
    private PositionData _positionData;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new();
      _positionDataMapper = _mocker.CreateInstance<PositionDataMapper>();

      _user = new()
      {
        Id = Guid.NewGuid(),
        PositionId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        IsActive = true
      };

      _users = new List<DbPositionUser>()
      {
        _user
      };

      _dbPosition = new()
      {
        Id = Guid.NewGuid(),
        Name = "Name",
        Description = "Description",
        IsActive = true,
        Users = _users
      };

      _positionData = new(_dbPosition.Id, _dbPosition.Name, _dbPosition.Users.Select(user => user.UserId).ToList());
    }

    [Test]
    public void DbPositionIsNotNull()
    {
      SerializerAssert.AreEqual(_positionData, _positionDataMapper.Map(_dbPosition));
    }

    [Test]
    public void DbPositionIsNull()
    {
      SerializerAssert.AreEqual(null, _positionDataMapper.Map(null));
    }
  }
}
