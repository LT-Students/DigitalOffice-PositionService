using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Mappers.Models;
using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.UnitTestKernel;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Mappers.UnitTests.Models
{
  public class PositionInfoMapperTests
  {
    private AutoMocker _mocker;
    private IPositionInfoMapper _positionInfoMapper;
    private DbPosition _dbPosition;
    private PositionInfo _positionInfo;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new();
      _positionInfoMapper = _mocker.CreateInstance<PositionInfoMapper>();

      _dbPosition = new()
      {
        Id = Guid.NewGuid(),
        Name = "Name",
        Description = "Description",
        IsActive = true
      };

      _positionInfo = new PositionInfo
      {
        Id = _dbPosition.Id, 
        Name = _dbPosition.Name,
        Description = _dbPosition.Description,
        IsActive = _dbPosition.IsActive
      };
    }

    [Test]
    public void DbPositionIsNotNull()
    {
      SerializerAssert.AreEqual(_positionInfo, _positionInfoMapper.Map(_dbPosition));
    }

    [Test]
    public void DbPositionIsNull()
    {
      SerializerAssert.AreEqual(null, _positionInfoMapper.Map(null));
    }
  }
}
