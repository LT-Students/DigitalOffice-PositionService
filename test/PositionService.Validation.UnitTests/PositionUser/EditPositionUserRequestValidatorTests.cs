using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.PositionService.Broker.Requests.Interfaces;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.PositionUser;
using LT.DigitalOffice.PositionService.Validation.PositionUser;
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
    public async Task ShouldValidateWhenRequestIsCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(true);

      (await _validator.TestValidateAsync(_request)).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task ShouldReturnErrorsWhenPositionNotExists()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(false);

      (await _validator.TestValidateAsync(_request)).ShouldHaveValidationErrorFor(nameof(EditPositionUserRequest.PositionId));
    }

    [Test]
    public async Task ShouldReturnErrorsWhenCheckUsersExistenceFailed()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesExistAsync(It.IsAny<Guid>()))
        .ReturnsAsync(true);

      _autoMocker
        .Setup<IUserService, Task<List<Guid>>>(x => x.CheckUsersExistenceAsync(It.IsAny<List<Guid>>(), default))
        .ReturnsAsync(new List<Guid> { });

      (await _validator.TestValidateAsync(_request)).ShouldHaveValidationErrorFor(nameof(EditPositionUserRequest.UserId));
    }
  }
}
