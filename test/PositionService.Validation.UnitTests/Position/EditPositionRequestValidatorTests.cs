using System;
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
    private IValidator<(Guid, JsonPatchDocument<EditPositionRequest>)> _validator;
    private ValueTuple<Guid, JsonPatchDocument<EditPositionRequest>> _valueTuple;
    private JsonPatchDocument<EditPositionRequest> _editUserRequest;
    private Guid _positionId;
    private string _longName = "12345678901234567890123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789";
    private string _longDescription = "12345678901234567890123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789" +
      "123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789123456789" +
      "123456789123456789123456789";

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _autoMocker = new AutoMocker();
      _validator = _autoMocker.CreateInstance<EditPositionRequestValidator>();

      _positionId = Guid.NewGuid();

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

      _valueTuple = new ValueTuple<Guid, JsonPatchDocument<EditPositionRequest>>(_positionId, _editUserRequest);
    }

    [SetUp]
    public void SetUp()
    {
      _autoMocker.GetMock<IPositionRepository>().Reset();
    }

    [Test]
    public async Task ShouldValidateWhenRequestIsCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid>()))
        .ReturnsAsync(false);

      Assert.AreEqual(true, (await _validator.ValidateAsync(_valueTuple)).IsValid);
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
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid>()))
        .ReturnsAsync(false);

      _validator.TestValidate(_valueTuple).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public void ShouldReturnErrorsWhenFirstNameIsTooLong()
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

      _validator.TestValidate(_valueTuple).ShouldHaveAnyValidationError();
    }

    [Test]
    public void ShouldReturnErrorsWhenFirstNameIsEmpty()
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
    public void ShouldReturnErrorsWhenLastNameIsTooLong()
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
    public void ShouldReturnErrorsWhenIsActiveNotCorrect()
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
    public void ShouldReturnErrorsWhenPositionNotUnique()
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
    public void ShouldReturnErrorsWhenOpNotCorrect()
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
