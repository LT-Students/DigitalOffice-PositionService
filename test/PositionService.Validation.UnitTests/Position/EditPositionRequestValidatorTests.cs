using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.TestHelper;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position;
using LT.DigitalOffice.PositionService.Validation.Position;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Validation.UnitTests.Position
{
  public class EditPositionRequestValidatorTests
  {
    private AutoMocker _autoMocker;
    private IValidator<JsonPatchDocument<EditPositionRequest>> _validator;
    private JsonPatchDocument<EditPositionRequest> _editUserRequest;
    private string _longName = "12345678901234567890123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789";
    private string _longDescription = "12345678901234567890123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789" +
      "123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789" +
      "123456789123456789123456789";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _autoMocker = new AutoMocker();
      _validator = _autoMocker.CreateInstance<EditPositionRequestValidator>();

      _editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
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
          true)
      }, new CamelCasePropertyNamesContractResolver());
    }

    [SetUp]
    public void SetUp()
    {
      _autoMocker.GetMock<IPositionRepository>().Reset();
    }

    [Test]
    public void ShouldValidateWhenRequestIsCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      _validator.TestValidate(_editUserRequest).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldValidateWhenDescriptionIsNull()
    {
      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Description)}",
          "",
          null)
      }, new CamelCasePropertyNamesContractResolver());

      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      _validator.TestValidate(_editUserRequest).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenFirstNameIsTooLong()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Name)}",
          "",
          _longName)
      }, new CamelCasePropertyNamesContractResolver());

      _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenFirstNameIsEmpty()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Name)}",
          "",
          "")
      }, new CamelCasePropertyNamesContractResolver());

      _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenLastNameIsTooLong()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Description)}",
          "",
          _longDescription)
      }, new CamelCasePropertyNamesContractResolver());

      _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenIsActiveNotCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.IsActive)}",
          "",
          "cat")
      }, new CamelCasePropertyNamesContractResolver());

      _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenPositionNotUnique()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(true);

      _validator.TestValidate(_editUserRequest).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldThrowValidationExceptionWhenOpNotCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "add",
          $"/{nameof(EditPositionRequest.Name)}",
          "",
          "name")
      }, new CamelCasePropertyNamesContractResolver());

      _validator.TestValidate(editUserRequest).ShouldHaveAnyValidationError();
    }
  }
}
