using System.Net;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Models.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Models;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Responses;
using System.Threading.Tasks;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using System;
using Moq.AutoMock;
using NUnit.Framework;
using LT.DigitalOffice.PositionService.Business.Commands.Position;
using Moq;
using LT.DigitalOffice.UnitTestKernel;
using System.Collections.Generic;

namespace LT.DigitalOffice.PositionService.Business.UnitTests
{
  public class GetPositionCommandTest
  {
    private AutoMocker _mocker;
    private IGetPositionCommand _getPositionCommand;
    private Guid _guid;
    private DbPosition dbPosition;
    private PositionInfo position;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new();
      _getPositionCommand = _mocker.CreateInstance<GetPositionCommand>();

      _guid = Guid.NewGuid();

      dbPosition = new();
      position = new();
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IPositionRepository>().Reset();
      _mocker.GetMock<IPositionInfoMapper>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
    }

    [Test]
    public async Task RepositoryReturnsNotNullElement()
    {
      OperationResultResponse<PositionInfo> result = new();
      result.Body = position;
      result.Status = OperationResultStatusType.FullSuccess;

      _mocker
        .Setup<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(_guid))
        .ReturnsAsync(dbPosition);

      _mocker
        .Setup<IPositionInfoMapper, PositionInfo>(x => x.Map(dbPosition))
        .Returns(position);

      SerializerAssert.AreEqual(result, (await _getPositionCommand.ExecuteAsync(_guid)));

      _mocker.Verify<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(_guid), Times.Once);
      _mocker.Verify<IResponseCreator, OperationResultResponse<PositionInfo>>(
        x => x.CreateFailureResponse<PositionInfo>(HttpStatusCode.NotFound, It.IsAny<List<string>>()), Times.Never);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(dbPosition), Times.Once);
    }

    [Test]
    public async Task RepositoryReturnsNullElement()
    {
      OperationResultResponse<PositionInfo> result = new(
        body: default,
        status: OperationResultStatusType.Failed,
        errors: new List<string>() { "Error message" });

      _mocker
        .Setup<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(_guid))
        .ReturnsAsync(It.IsAny<DbPosition>);

      _mocker
       .Setup<IResponseCreator, OperationResultResponse<PositionInfo>>(x =>
         x.CreateFailureResponse<PositionInfo>(HttpStatusCode.NotFound, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _getPositionCommand.ExecuteAsync(_guid));

      _mocker.Verify<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(_guid), Times.Once);
      _mocker.Verify<IResponseCreator, OperationResultResponse<PositionInfo>>(
        x => x.CreateFailureResponse<PositionInfo>(HttpStatusCode.NotFound, It.IsAny<List<string>>()), Times.Once);
      _mocker.Verify<IPositionInfoMapper, PositionInfo>(x => x.Map(dbPosition), Times.Never);
    }
  }
}
