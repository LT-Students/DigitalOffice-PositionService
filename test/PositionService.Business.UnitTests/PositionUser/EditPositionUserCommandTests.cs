using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.PositionUser;
using LT.DigitalOffice.PositionService.Business.Commands.PositionUser.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Mappers.Db.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Validation.PositionUser.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Business.UnitTests.PositionUser
{
  public class EditPositionUserCommandTests
  {
    private AutoMocker _mocker;
    private IEditPositionUserCommand _editPositionUserCommand;
    private EditPositionUserRequest _request;
    private DbPositionUser _dbPositionUser;

    private void Verifiable(
      Times accessValidatorTimes,
      Times responseCreatorTimes,
      Times positionUserRepositoryDoesExistTimes,
      Times positionUserRepositoryEditTimes,
      Times positionUserRepositoryCreateTimes,
      Times dbPositionUserMapperTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IPositionUserRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()), positionUserRepositoryDoesExistTimes);
      _mocker.Verify<IPositionUserRepository, Task<Guid?>>(x => x.EditAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), positionUserRepositoryEditTimes);
      _mocker.Verify<IPositionUserRepository, Task<Guid?>>(x => x.CreateAsync(It.IsAny<DbPositionUser>()), positionUserRepositoryCreateTimes);
      _mocker.Verify<IDbPositionUserMapper, DbPositionUser>(x => x.Map(It.IsAny<EditPositionUserRequest>()), dbPositionUserMapperTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new();
      _editPositionUserCommand = _mocker.CreateInstance<EditPositionUserCommand>();

      _request = new()
      {
        UserId = Guid.NewGuid(),
        PositionId = Guid.NewGuid()
      };

      _dbPositionUser = new()
      {
        Id = Guid.NewGuid(),
        PositionId = Guid.NewGuid()
      };

      ValidationResult validationResult = new ValidationResult();

      _mocker
        .Setup<IEditPositionUserRequestValidator, Task<ValidationResult>>(x => x.ValidateAsync(It.IsAny<EditPositionUserRequest>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(validationResult);
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IPositionUserRepository>().Reset();
      _mocker.GetMock<IDbPositionUserMapper>().Reset();
    }

    [Test]
    public async Task HasNotEnoughRightsTest()
    {
      OperationResultResponse<bool> result = new(
        body: false,
        errors: new List<string>() { "Not enough rights." });

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemovePositions))
        .ReturnsAsync(false);

      _mocker
       .Setup<IResponseCreator, OperationResultResponse<bool>>(x =>
         x.CreateFailureResponse<bool>(HttpStatusCode.Forbidden, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _editPositionUserCommand.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        positionUserRepositoryDoesExistTimes: Times.Never(),
        positionUserRepositoryEditTimes: Times.Never(),
        positionUserRepositoryCreateTimes: Times.Never(),
        dbPositionUserMapperTimes: Times.Never());
    }

    [Test]
    public async Task UserExistsTest()
    {
      OperationResultResponse<bool> result = new OperationResultResponse<bool>(
          body: true,
          errors: new List<string>());
        
      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemovePositions))
        .ReturnsAsync(true);

      _mocker
        .Setup<IPositionUserRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(true);

      _mocker
        .Setup<IPositionUserRepository, Task<Guid?>>(x => x.EditAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
        .ReturnsAsync(Guid.NewGuid());

      _mocker
        .Setup<IPositionUserRepository, Task<DbPositionUser>>(x => x.GetAsync(It.IsAny<Guid>()))
        .ReturnsAsync(_dbPositionUser);

      _mocker
       .Setup<IResponseCreator, OperationResultResponse<bool>>(x =>
         x.CreateFailureResponse<bool>(HttpStatusCode.NotFound, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _editPositionUserCommand.ExecuteAsync(_request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        positionUserRepositoryDoesExistTimes: Times.Once(),
        positionUserRepositoryEditTimes: Times.Once(),
        positionUserRepositoryCreateTimes: Times.Never(),
        dbPositionUserMapperTimes: Times.Never());
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task UserNotExistsTest(bool isPositionId)
    {
      OperationResultResponse<bool> result;

      EditPositionUserRequest request;

      if (isPositionId)
      {
        result = new(
          body: true,
          errors: new List<string>());

        request = new EditPositionUserRequest
        {
          UserId = Guid.NewGuid(),
          PositionId = Guid.NewGuid()
        };
      }
      else
      {
        result = new(
          body: false,
          errors: new List<string>() { "Request is not correct." });

        request = new EditPositionUserRequest
        {
          UserId = Guid.NewGuid()
        };
      }

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemovePositions))
        .ReturnsAsync(true);

      _mocker
        .Setup<IPositionUserRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(false);

      _mocker
        .Setup<IDbPositionUserMapper, DbPositionUser>(x => x.Map(request))
        .Returns(_dbPositionUser);

      _mocker
        .Setup<IPositionUserRepository, Task<Guid?>>(x => x.CreateAsync(It.IsAny<DbPositionUser>()))
        .ReturnsAsync(Guid.NewGuid());

      _mocker
        .Setup<IPositionUserRepository, Task<DbPositionUser>>(x => x.GetAsync(It.IsAny<Guid>()))
        .ReturnsAsync(_dbPositionUser);

      _mocker
       .Setup<IResponseCreator, OperationResultResponse<bool>>(x =>
         x.CreateFailureResponse<bool>(HttpStatusCode.BadRequest, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _editPositionUserCommand.ExecuteAsync(request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        positionUserRepositoryDoesExistTimes: Times.Once(),
        positionUserRepositoryEditTimes: Times.Never(),
        positionUserRepositoryCreateTimes: isPositionId ? Times.Once() : Times.Never(),
        dbPositionUserMapperTimes: isPositionId ? Times.Once() : Times.Never());
    }
  }
}
