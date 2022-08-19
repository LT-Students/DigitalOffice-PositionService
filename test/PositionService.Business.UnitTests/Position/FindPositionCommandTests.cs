using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.Kernel.Validators.Interfaces;
using LT.DigitalOffice.PositionService.Business.Commands.Position;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Business.UnitTests
{
  public class FindPositionCommandTests
  {
    private AutoMocker _mocker;
    private IFindPositionsCommand _command;
    private List<PositionInfo> _positionInfo;
    private List<DbPosition> _dbPositions;
    private FindPositionsFilter _filter;
    List<Guid> _ids = new() { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new AutoMocker();
      _command = _mocker.CreateInstance<FindPositionsCommand>();
      _filter = new FindPositionsFilter();

      _dbPositions = new List<DbPosition>()
      {
        new DbPosition()
        {
          Id = _ids[0],
          Name = "TestName1",
          Description = "TestDescription1",
          IsActive = true,
          CreatedBy = Guid.NewGuid(),
          CreatedAtUtc = DateTime.UtcNow,
        },

        new DbPosition()
        {
          Id = _ids[1],
          Name = "TestName2",
          Description = "TestDescription2",
          IsActive = true,
          CreatedBy = Guid.NewGuid(),
          CreatedAtUtc = DateTime.UtcNow,
        },

        new DbPosition()
        {
          Id = _ids[2],
          Name = "TestName3",
          Description = "TestDescription3",
          IsActive = true,
          CreatedBy = Guid.NewGuid(),
          CreatedAtUtc = DateTime.UtcNow,
        },
      };

      _positionInfo = new List<PositionInfo>();

      foreach (var position in _dbPositions)
      {
        _positionInfo.Add(
          new PositionInfo()
          {
            Id = position.Id,
            Name = position.Name,
            Description = position.Description,
            IsActive = position.IsActive
          });
      }
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IPositionRepository>().Reset();
      _mocker.GetMock<IPositionInfoMapper>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IBaseFindFilterValidator>().Reset();
    }

    [Test]
    public async Task ShouldReturnListPositionInfoAsync()
    {
      _mocker
        .Setup<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()))
        .ReturnsAsync((_dbPositions, _dbPositions.Count));

      _mocker
        .SetupSequence<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()))
        .Returns(_positionInfo[0])
        .Returns(_positionInfo[1])
        .Returns(_positionInfo[2]);

      SerializerAssert.AreEqual(_positionInfo, (await _command.ExecuteAsync(_filter)).Body);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Exactly(3));
    }

    [Test]
    public async Task ShouldReturnListOfInactivePositionInfoAsync()
    {
      Guid positionId = Guid.NewGuid();
      List<PositionInfo> result = new List<PositionInfo>
      {
        new PositionInfo()
        {
          Id = positionId,
          Name = "InactiveTestName",
          Description = "InactiveTestDescription",
          IsActive = false,
        }
      };

      List<DbPosition> position = new List<DbPosition>
      {
        new DbPosition()
        {
          Id = positionId,
          Name = "InactiveTestName",
          Description = "InactiveTestDescription",
          IsActive = false,
          CreatedBy = Guid.NewGuid(),
          CreatedAtUtc = DateTime.UtcNow,
        }
      };

      int totalCount = 1;

      _filter = new FindPositionsFilter()
      {
        IsActive = false,
      };

      _mocker
        .Setup<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(_filter))
        .ReturnsAsync((position, totalCount));

      _mocker
        .Setup<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()))
        .Returns(result[0]);

      SerializerAssert.AreEqual(result, (await _command.ExecuteAsync(_filter)).Body);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Once);
    }

    [Test]
    public async Task ShouldReturnListSortPositionInfoAsync()
    {
      _filter = new FindPositionsFilter()
      {
        IsActive = true,
        IsAscendingSort = true,
      };

      _mocker
        .Setup<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(_filter))
        .ReturnsAsync((_dbPositions, _dbPositions.Count));

      _mocker
        .SetupSequence<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()))
        .Returns(_positionInfo[0])
        .Returns(_positionInfo[1])
        .Returns(_positionInfo[2]);

      SerializerAssert.AreEqual(_positionInfo, (await _command.ExecuteAsync(_filter)).Body);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Exactly(3));
    }

    [Test]
    public async Task ShouldReturnListDescendingSortPositionInfoAsync()
    {
      List<PositionInfo> result = new List<PositionInfo>() { _positionInfo[2], _positionInfo[1], _positionInfo[0] };
      _filter = new FindPositionsFilter()
      {
        IsActive = true,
        IsAscendingSort = false,
      };

      _mocker
        .Setup<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(_filter))
        .ReturnsAsync((_dbPositions, _dbPositions.Count));

      _mocker
        .SetupSequence<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()))
        .Returns(_positionInfo[2])
        .Returns(_positionInfo[1])
        .Returns(_positionInfo[0]);

      SerializerAssert.AreEqual(result, (await _command.ExecuteAsync(_filter)).Body);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Exactly(3));
    }

    [Test]
    public async Task ShouldReturnNullAsync()
    {
      List<DbPosition> result = null;
      List<DbPosition> dblist = null;
      int totalCount = 0;

      _mocker
        .Setup<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()))
        .ReturnsAsync((dblist, totalCount));

      SerializerAssert.AreEqual(result, (await _command.ExecuteAsync(_filter)).Body);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Never);
    }

    [Test]
    public async Task ShouldReturnFailedResponseWhenValidationIsFailedAsync()
    {
      FindResultResponse<PositionInfo> result = new(
        body: default,
        totalCount: 0,
        status: OperationResultStatusType.Failed,
        errors: new List<string>() { "Error message" });

      _mocker
       .Setup<IBaseFindFilterValidator, bool>(x =>
         x.Validate(It.IsAny<IValidationContext>()).IsValid)
       .Returns(false);

      _mocker
       .Setup<IResponseCreator, FindResultResponse<PositionInfo>>(x =>
         x.CreateFailureFindResponse<PositionInfo>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _command.ExecuteAsync(_filter));

      _mocker.Verify<IResponseCreator, FindResultResponse<PositionInfo>>(
        x => x.CreateFailureFindResponse<PositionInfo>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()), Times.Once);
      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Never);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Never);
    }
  }
}
