using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.Kernel.BrokerSupport.Broker;
using LT.DigitalOffice.Models.Broker.Common;
using LT.DigitalOffice.PositionService.Broker.Requests.Interfaces;
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
      _autoMocker
        .Setup<IUserService, Task<List<Guid>>>(x => x.CheckUsersExistenceAsync(It.IsAny<List<Guid>>(), default))
        .ReturnsAsync(new List<Guid> { Guid.NewGuid() });
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
    public void ShouldReturnErrorsWhenPositionNotExists()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(false);

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldReturnErrorsWhenCheckUsersExistenceFailed()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(true);

      _autoMocker
        .Setup<IUserService, Task<List<Guid>>>(x => x.CheckUsersExistenceAsync(It.IsAny<List<Guid>>(), default))
        .ReturnsAsync(new List<Guid> { });

      _validator.TestValidate(_request).ShouldHaveAnyValidationError();
    }
  }
}
