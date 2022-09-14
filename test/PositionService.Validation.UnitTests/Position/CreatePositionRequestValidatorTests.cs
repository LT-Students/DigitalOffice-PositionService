using System;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Validation.UnitTests.Position
{
  public class CreatePositionRequestValidatorTests
  {
    private AutoMocker _autoMocker;
    private IValidator<CreatePositionRequest> _validator;
    private string _longName = "12345678901234567890123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789";
    private string _longDescription = "12345678901234567890123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789" +
      "123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789" +
      "123456789123456789123456789";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _autoMocker = new AutoMocker();
      _validator = _autoMocker.CreateInstance<CreatePositionRequestValidator>();
    }

    [SetUp]
    public void SetUp()
    {
      _autoMocker.GetMock<IPositionRepository>().Reset();
    }

    [TestCase("Description")]
    [TestCase(null)]
    public void ShouldNotReturnErrors(string description)
    {
      CreatePositionRequest request = new CreatePositionRequest()
      {
        Name = "Name",
        Description = description
      };

      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), default))
        .ReturnsAsync(false);

      _validator.TestValidate(request).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldReturnErrorsWhenDescriptionTooLong()
    {
      CreatePositionRequest request = new CreatePositionRequest()
      {
        Name = "Name",
        Description = _longDescription
      };

      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), default))
        .ReturnsAsync(false);

      _validator.TestValidate(request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldReturnErrorsWhenNameTooLong()
    {
      CreatePositionRequest request = new CreatePositionRequest()
      {
        Name = _longName,
        Description = "Description"
      };

      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), default))
        .ReturnsAsync(false);

      _validator.TestValidate(request).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldReturnErrorsWhenPositionNotUnique()
    {
      CreatePositionRequest request = new CreatePositionRequest()
      {
        Name = "Name",
        Description = "Description"
      };

      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), default))
        .ReturnsAsync(true);

      _validator.TestValidate(request).ShouldHaveAnyValidationError();
    }
  }
}
