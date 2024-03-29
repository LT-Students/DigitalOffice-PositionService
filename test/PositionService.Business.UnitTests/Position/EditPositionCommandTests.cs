﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FluentValidation.Results;
using LT.DigitalOffice.Kernel.BrokerSupport.AccessValidatorEngine.Interfaces;
using LT.DigitalOffice.Kernel.Constants;
using LT.DigitalOffice.Kernel.Helpers.Interfaces;
using LT.DigitalOffice.Kernel.Responses;
using LT.DigitalOffice.PositionService.Business.Commands.Position;
using LT.DigitalOffice.PositionService.Business.Commands.Position.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position.Interfaces;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Business.UnitTests
{
  public class EditPositionCommandTests
  {
    private AutoMocker _mocker;
    private IEditPositionCommand _editPositionCommand;
    private JsonPatchDocument<EditPositionRequest> _request;
    private JsonPatchDocument<EditPositionRequest> _request2;
    private DbPosition _dbPosition;

    private void Verifiable(
      Times accessValidatorTimes,
      Times requestValidatorTimes,
      Times responseCreatorTimes,
      Times positionRepositoryGetTimes,
      Times positionRepositoryContainsUsersTimes,
      Times positionRepositoryEditTimes)
    {
      _mocker.Verify<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(It.IsAny<int>()), accessValidatorTimes);
      _mocker.Verify<IEditPositionRequestValidator, Task<ValidationResult>>(
        x => x.ValidateAsync(It.IsAny<ValueTuple<Guid, JsonPatchDocument<EditPositionRequest>>>(), default),
        requestValidatorTimes);
      _mocker.Verify<IResponseCreator, OperationResultResponse<bool>>(
        x => x.CreateFailureResponse<bool>(It.IsAny<HttpStatusCode>(), It.IsAny<List<string>>()), responseCreatorTimes);
      _mocker.Verify<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(It.IsAny<Guid>()), positionRepositoryGetTimes);
      _mocker.Verify<IPositionRepository, Task<bool>>(x => x.ContainsUsersAsync(It.IsAny<Guid>()), positionRepositoryContainsUsersTimes);
      _mocker.Verify<IPositionRepository, Task<bool>>(x => x.EditAsync(It.IsAny<DbPosition>(), It.IsAny<JsonPatchDocument<DbPosition>>()), positionRepositoryEditTimes);

      _mocker.Resolvers.Clear();
    }

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _mocker = new();
      _editPositionCommand = _mocker.CreateInstance<EditPositionCommand>();

      _dbPosition = new();

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
              "True")
      }, new CamelCasePropertyNamesContractResolver());

      _request2 = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
          new Operation<EditPositionRequest>(
              "replace",
              $"/{nameof(EditPositionRequest.IsActive)}",
              "",
              "False")
      }, new CamelCasePropertyNamesContractResolver());
    }

    [SetUp]
    public void SetUp()
    {
      _mocker.GetMock<IAccessValidator>().Reset();
      _mocker.GetMock<IResponseCreator>().Reset();
      _mocker.GetMock<IPositionRepository>().Reset();
      _mocker.GetMock<IEditPositionRequestValidator>().Reset();

      _mocker
        .Setup<IEditPositionRequestValidator, Task<ValidationResult>>(
          x => x.ValidateAsync(It.IsAny<ValueTuple<Guid, JsonPatchDocument<EditPositionRequest>>>(), default))
        .ReturnsAsync(new ValidationResult());
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

      SerializerAssert.AreEqual(result, await _editPositionCommand.ExecuteAsync(It.IsAny<Guid>(), _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        requestValidatorTimes: Times.Never(),
        responseCreatorTimes: Times.Never(),
        positionRepositoryGetTimes: Times.Never(),
        positionRepositoryContainsUsersTimes: Times.Never(),
        positionRepositoryEditTimes: Times.Never());
    }

    [Test]
    public async Task RequestValidationFailedTest()
    {
      OperationResultResponse<bool> result = new(
        body: false,
        errors: new List<string>() { "Name is too long." });

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemovePositions))
        .ReturnsAsync(true);

      _mocker
        .Setup<IEditPositionRequestValidator, Task<ValidationResult>>(
          x => x.ValidateAsync(It.IsAny<ValueTuple<Guid, JsonPatchDocument<EditPositionRequest>>>(), default))
        .ReturnsAsync(new ValidationResult(new List<ValidationFailure>() { new("Name", "Name is too long.") }));

      _mocker
       .Setup<IResponseCreator, OperationResultResponse<bool>>(x =>
         x.CreateFailureResponse<bool>(HttpStatusCode.NotFound, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _editPositionCommand.ExecuteAsync(It.IsAny<Guid>(), _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        requestValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        positionRepositoryGetTimes: Times.Never(),
        positionRepositoryContainsUsersTimes: Times.Never(),
        positionRepositoryEditTimes: Times.Never());
    }

    [Test]
    public async Task PositionIsNullTest()
    {
      OperationResultResponse<bool> result = new(
        body: false,
        errors: new List<string>() { "Nothing found on request." });

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemovePositions))
        .ReturnsAsync(true);

      _mocker
        .Setup<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(It.IsAny<Guid>()))
        .ReturnsAsync(It.IsAny<DbPosition>);

      _mocker
       .Setup<IResponseCreator, OperationResultResponse<bool>>(x =>
         x.CreateFailureResponse<bool>(HttpStatusCode.NotFound, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _editPositionCommand.ExecuteAsync(It.IsAny<Guid>(), _request));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        requestValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        positionRepositoryGetTimes: Times.Once(),
        positionRepositoryContainsUsersTimes: Times.Never(),
        positionRepositoryEditTimes: Times.Never());
    }

    [Test]
    public async Task PositionContainsUserTest()
    {
      OperationResultResponse<bool> result = new(
        body: false,
        errors: new List<string>() { "The position contains users. Please change the users' position." });

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemovePositions))
        .ReturnsAsync(true);

      _mocker
        .Setup<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(It.IsAny<Guid>()))
        .ReturnsAsync(_dbPosition);

      _mocker
        .Setup<IPositionRepository, Task<bool>>(x => x.ContainsUsersAsync(It.IsAny<Guid>()))
        .ReturnsAsync(true);

      _mocker
       .Setup<IResponseCreator, OperationResultResponse<bool>>(x =>
         x.CreateFailureResponse<bool>(HttpStatusCode.Conflict, It.IsAny<List<string>>()))
       .Returns(result);

      SerializerAssert.AreEqual(result, await _editPositionCommand.ExecuteAsync(It.IsAny<Guid>(), _request2));

      Verifiable(
        accessValidatorTimes: Times.Once(),
        requestValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        positionRepositoryGetTimes: Times.Once(),
        positionRepositoryContainsUsersTimes: Times.Once(),
        positionRepositoryEditTimes: Times.Never());
    }

    [TestCase(true)]
    [TestCase(false)]
    public async Task ResultTest(bool resultSuccess)
    {
      OperationResultResponse<bool> expected;

      if (resultSuccess)
      {
        expected = new(
          body: true,
          errors: new List<string>());

        _mocker
          .Setup<IPositionRepository, Task<bool>>(x => x.EditAsync(It.IsAny<DbPosition>(), It.IsAny<JsonPatchDocument<DbPosition>>()))
          .ReturnsAsync(true);
      }
      else
      {
        expected = new(
          body: false,
          errors: new List<string>());

        _mocker
          .Setup<IPositionRepository, Task<bool>>(x => x.EditAsync(It.IsAny<DbPosition>(), It.IsAny<JsonPatchDocument<DbPosition>>()))
          .ReturnsAsync(false);
      }

      _mocker
        .Setup<IAccessValidator, Task<bool>>(x => x.HasRightsAsync(Rights.AddEditRemovePositions))
        .ReturnsAsync(true);

      _mocker
        .Setup<IPositionRepository, Task<DbPosition>>(x => x.GetAsync(It.IsAny<Guid>()))
        .ReturnsAsync(_dbPosition);

      _mocker
        .Setup<IPositionRepository, Task<bool>>(x => x.ContainsUsersAsync(It.IsAny<Guid>()))
        .ReturnsAsync(false);

      object result = await _editPositionCommand.ExecuteAsync(It.IsAny<Guid>(), _request);

      SerializerAssert.AreEqual(expected, result);

      Verifiable(
        accessValidatorTimes: Times.Once(),
        requestValidatorTimes: Times.Once(),
        responseCreatorTimes: Times.Never(),
        positionRepositoryGetTimes: Times.Once(),
        positionRepositoryContainsUsersTimes: Times.Never(),
        positionRepositoryEditTimes: Times.Once());
    }
  }
}
