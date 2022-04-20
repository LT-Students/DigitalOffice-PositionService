using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using FluentValidation;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.FluentValidationExtensions;
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
    private int _totalCount;
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
        }
      };

      _totalCount = 3;
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
    }

    [Test]
    public async Task ShouldReturnListPositionInfoAsync()
    {
      var result = _positionInfo;

      _mocker
        .Setup<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()))
        .ReturnsAsync((_dbPositions, _totalCount));

      _mocker
        .SetupSequence<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()))
        .Returns(_positionInfo[0])
        .Returns(_positionInfo[1])
        .Returns(_positionInfo[2]);

      SerializerAssert.AreEqual(result, (await _command.ExecuteAsync(_filter)).Body);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Exactly(3));
    }

    [Test]
    public async Task ShouldReturnNullAsync()
    {
      List<DbPosition> dblist = null;
      List<DbPosition> result = null;
      _totalCount = 0;

      _mocker
        .Setup<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()))
        .ReturnsAsync((dblist, _totalCount));

      SerializerAssert.AreEqual(result, (await _command.ExecuteAsync(_filter)).Body);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Never);
    }

    [Test]
    public async Task ShouldReturnFailedResponseWhenValidationIsFailedAsync()
    {
      List<string> res = new List<string>() { "Error message" };

      FindResultResponse<PositionInfo> result = new(      
        body: default,
        totalCount: 0,
        status: OperationResultStatusType.Failed,
        errors: new List<string>() { "Error message" });

      _mocker
       .Setup<IBaseFindFilterValidator, bool>(x =>
         x.ValidateCustom(It.IsAny<FindPositionsFilter>(), out res))
       .Returns(false);

      _mocker
       .Setup<IResponseCreator, FindResultResponse<PositionInfo>>(x =>
         x.CreateFailureFindResponse<PositionInfo>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
       .Returns(new FindResultResponse<PositionInfo>()
       {
        Body = default,
        TotalCount = 0,
        Status = OperationResultStatusType.Failed,
        Errors = new List<string>() { "Error message" }
       });

      SerializerAssert.AreEqual(result, (await _command.ExecuteAsync(_filter)).Errors);

      _mocker.Verify<IResponseCreator, FindResultResponse<PositionInfo>>(
        x => x.CreateFailureFindResponse<PositionInfo>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()), Times.Once);

      _mocker.Verify<IPositionRepository, Task<(List<DbPosition>, int totalCount)>>(x => x.FindAsync(It.IsAny<FindPositionsFilter>()), Times.Never);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(It.IsAny<DbPosition>()), Times.Never);
    }
  }
}
