using System;
using System.Collections.Generic;
using System.Linq;
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

      (await _validator.TestValidateAsync(_valueTuple)).ShouldNotHaveAnyValidationErrors();
    }

    [Test]
    public async Task ShouldReturnErrorsWhenOpIsNotCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "add",
          $"/{nameof(EditPositionRequest.Name)}",
          "",
          "name")
      }, new CamelCasePropertyNamesContractResolver());

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.Name));
    }

    [Test]
    public async Task ShouldReturnErrorsWhenPathIsNotCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/IncorrectPath",
          "",
          "value")
      }, new CamelCasePropertyNamesContractResolver());

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(editUserRequest.Operations.FirstOrDefault().path);
    }

    [Test]
    public async Task ShouldReturnErrorsWhenDescriptionIsTooLong()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Description)}",
          "",
          _longDescription)
      }, new CamelCasePropertyNamesContractResolver());

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.Description));
    }

    [Test]
    public async Task ShouldValidateWhenDescriptionIsNull()
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

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.Description));
    }

    [Test]
    public async Task ShouldReturnErrorsWhenNameIsTooLong()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Name)}",
          "",
          _longName)
      }, new CamelCasePropertyNamesContractResolver());

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.Name));
    }

    [Test]
    public async Task ShouldReturnErrorsWhenNameIsEmpty()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Name)}",
          "",
          "")
      }, new CamelCasePropertyNamesContractResolver());

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.Name));
    }

    [Test]
    public async Task ShouldReturnErrorsWhenNameIsNull()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.Name)}",
          "",
          null)
      }, new CamelCasePropertyNamesContractResolver());

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.Name));
    }

    [Test]
    public async Task ShouldReturnErrorsWhenNameIsNotUnique()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(true);

      (await _validator.TestValidateAsync((_positionId, _editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.Name));
    }

    [Test]
    public async Task ShouldReturnErrorsWhenIsActiveNotCorrect()
    {
      _autoMocker
        .Setup<IPositionRepository, Task<bool>>(x => x.DoesNameExistAsync(It.IsAny<string>(), It.IsAny<Guid?>()))
        .ReturnsAsync(false);

      JsonPatchDocument<EditPositionRequest> editUserRequest = new JsonPatchDocument<EditPositionRequest>(new List<Operation<EditPositionRequest>>
      {
        new Operation<EditPositionRequest>(
          "replace",
          $"/{nameof(EditPositionRequest.IsActive)}",
          "",
          "cat")
      }, new CamelCasePropertyNamesContractResolver());

      (await _validator.TestValidateAsync((_positionId, editUserRequest))).ShouldHaveValidationErrorFor(nameof(EditPositionRequest.IsActive));
    }
  }
}
