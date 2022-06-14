using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Validation.PositionUser;
using MassTransit;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Validation.UnitTests.PositionUser
{
  public class EditPositionUserRequestValidatorTests
  {
    private AutoMocker _autoMocker;
    private IValidator<EditPositionUserRequest> _validator;
    private EditPositionUserRequest _request;
    private Mock<Response<IOperationResult<ICheckUsersExistence>>> _operationResultBroker;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _autoMocker = new AutoMocker();
      _validator = _autoMocker.CreateInstance<EditPositionUserRequestValidator>();

      _request = new EditPositionUserRequest
      {
        UserId = Guid.NewGuid(),
        PositionId = Guid.NewGuid()
      };
    }

    [SetUp]
    public void SetUp()
    {
      _autoMocker.GetMock<IPositionRepository>().Reset();

      List<Guid> userIds = new() { Guid.NewGuid() };

      Mock<ICheckUsersExistence> usersResponse = new Mock<ICheckUsersExistence>();
      usersResponse.Setup(x => x.UserIds).Returns(userIds);

      _operationResultBroker = new Mock<Response<IOperationResult<ICheckUsersExistence>>>();
      _operationResultBroker.Setup(x => x.Message.Body).Returns(usersResponse.Object);
      _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(true);
      _operationResultBroker.Setup(x => x.Message.Errors).Returns(new List<string> { "Some errors" });

      _autoMocker
        .Setup<IRequestClient<ICheckUsersExistence>, Task<Response<IOperationResult<ICheckUsersExistence>>>>(
          x => x.GetResponse<IOperationResult<ICheckUsersExistence>>(
            It.IsAny<object>(), default, It.IsAny<RequestTimeout>()))
        .ReturnsAsync(_operationResultBroker.Object);
    }

    [Test]
    public void ShouldValidateWhenRequestIsCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(true);

      _validator.TestValidate(_request).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenPositionNotExists()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(false);

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenCheckUsersExistenceFailed()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(true);

      _operationResultBroker.Setup(x => x.Message.IsSuccess).Returns(false);

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }
  }
}
