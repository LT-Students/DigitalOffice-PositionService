using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Data.Provider;
using LT.DigitalOffice.PositionService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Data.UnitTests
{
  public class PositionRepositoryTests
  {
    private IDataProvider _provider;
    private IPositionRepository _repository;
    private DbContextOptions<PositionServiceDbContext> _dbContext;

    private DbPosition _position1;
    private DbPosition _position2;
    private DbPosition _positionWithUser;
    private DbPosition _deactivatedPosition;
    private DbPosition _deactivatedPosition2;
    private Guid _creatorId = Guid.NewGuid();

    private DbPositionUser _user;

    private AutoMocker _mocker;
    private IHttpContextAccessor _contextAccessor;

    [SetUp]
    public void SetUp()
    {
      _position1 = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName1",
        Description = "TestDescription1",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      _position2 = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName2",
        Description = "TestDescription2",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      _deactivatedPosition = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName3",
        Description = "TestDescription3",
        IsActive = false,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      _positionWithUser = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName4",
        Description = "TestDescription3",
        IsActive = true,
        CreatedBy = _creatorId,
        CreatedAtUtc = DateTime.UtcNow,
      };

      _deactivatedPosition2 = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName5",
        Description = "TestDescription5",
        IsActive = false,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      _user = new DbPositionUser()
      {
        Id = Guid.NewGuid(),
        PositionId = _positionWithUser.Id,
        UserId = Guid.NewGuid(),
        IsActive = true,
        CreatedBy = _creatorId,
        CreatedAtUtc = DateTime.UtcNow
      };

      CreateMemoryDb();

      _provider.Positions.AddRange(_position1);
      _provider.Positions.AddRange(_position2);
      _provider.Positions.AddRange(_deactivatedPosition);
      _provider.Positions.AddRange(_positionWithUser);
      _provider.PositionsUsers.AddRange(_user);
      _provider.Save();

      _mocker.GetMock<IHttpContextAccessor>().Reset();
    }

    public void CreateMemoryDb()
    {
      _mocker = new AutoMocker();
      _contextAccessor = _mocker.CreateInstance<HttpContextAccessor>();
      _dbContext = new DbContextOptionsBuilder<PositionServiceDbContext>()
                 .UseInMemoryDatabase(databaseName: "PositionServiceTes")
                 .Options;

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _creatorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      _provider = new PositionServiceDbContext(_dbContext);
      _repository = new PositionRepository(_provider, _contextAccessor);
    }

    #region AddPosition

    [Test]
    public async Task ShouldAddPositionAsync()
    {
      DbPosition position = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName",
        Description = "TestDescription",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      SerializerAssert.AreEqual(position.Id, await _repository.CreateAsync(position));
    }

    [Test]
    public async Task ShouldReturnNullForAddNullPositionAsync()
    {
      SerializerAssert.AreEqual(null, await _repository.CreateAsync(null));
    }

    #endregion

    #region GetPosition

    [Test]
    public async Task ShouldReturnPositionForIdAsync()
    {
      SerializerAssert.AreEqual(_position1, await _repository.GetAsync(_position1.Id));
    }

    [Test]
    public async Task ShouldReturnNullPositionForIdAsync()
    {
      SerializerAssert.AreEqual(null, await _repository.GetAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task ShouldReturnPositionsforRequest()
    {
      List<Guid> ids = new List<Guid>() { _user.UserId };

      _mocker = new AutoMocker();
      _mocker
        .Setup<IGetPositionsRequest, List<Guid>>(x => x.UsersIds)
        .Returns(ids);

      List<DbPosition> expectedResponse = new List<DbPosition>() { _positionWithUser };

      Assert.AreEqual(expectedResponse, await _repository.GetAsync(_mocker.GetMock<IGetPositionsRequest>().Object));
    }

    [Test]
    public async Task ShouldReturnListPositionsForIdsAsync()
    {
      List<Guid> ids = new List<Guid>() { _position1.Id, _position2.Id };
      List<DbPosition> positions = new List<DbPosition>() { _position1, _position2 };

      SerializerAssert.AreEqual(positions, await _repository.GetAsync(ids));
    }

    [Test]
    public async Task ShouldReturnNullPositionsForIdsAsync()
    {
      List<Guid> ids = new List<Guid>() { };
      List<DbPosition> positions = new List<DbPosition>() { };

      SerializerAssert.AreEqual(positions, await _repository.GetAsync(ids));
    }

    [Test]
    public async Task ShouldReturnPositionForIdsAsync()
    {
      List<Guid> ids = new List<Guid>() { _position1.Id, Guid.NewGuid() };
      List<DbPosition> positions = new List<DbPosition>() { _position1 };

      SerializerAssert.AreEqual(positions, await _repository.GetAsync(ids));
    }

    #endregion

    #region FindPositions

    [Test]
    public async Task ShouldReturnListOfActivePositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _position1, _position2, _positionWithUser };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsActive = true,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 3);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldReturnInactivePositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _deactivatedPosition, _deactivatedPosition2 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsActive = false,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 2);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldReturnListOfAllPositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition> { _position1, _position2, _positionWithUser, _deactivatedPosition, _deactivatedPosition2 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 5);
      var response = await _repository.FindAsync(filter);

      SerializerAssert.AreEqual(expectedResponse, response);
    }

    [Test]
    public async Task ShouldReturnListOfSortAllPositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _position1, _position2, _deactivatedPosition, _positionWithUser, _deactivatedPosition2 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsAscendingSort = true,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 5);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldReturnListOfDescendingSortAllPositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _deactivatedPosition2, _positionWithUser, _deactivatedPosition, _position2, _position1 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsAscendingSort = false,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 5);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldReturnListOfSortActivePositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _position1, _position2, _positionWithUser };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsAscendingSort = true,
        IsActive = true,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 3);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldReturnListOfDescendingSortActivePositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _positionWithUser, _position2, _position1 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsAscendingSort = false,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 3);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldReturnListOfSortInactivePositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _deactivatedPosition, _deactivatedPosition2 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsAscendingSort = true,
        IsActive = false,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 2);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldReturnListOfDescendingSortInactivePositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _deactivatedPosition2, _deactivatedPosition };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IsAscendingSort = false,
        IsActive = false,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 2);

      SerializerAssert.AreEqual(expectedResponse, await _repository.FindAsync(filter));
    }

    [Test]
    public async Task ShouldSkipOnePositionAndTakeOneAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _position2 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        SkipCount = 1,
        TakeCount = 1,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 1);
      var response = await _repository.FindAsync(filter);

      SerializerAssert.AreEqual(expectedResponse, response);
    }

    #endregion

    #region ContainsUsers

    [Test]
    public async Task ShouldReturnTrueForPositionsUserAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.ContainsUsersAsync(_positionWithUser.Id));
    }

    [Test]
    public async Task ShouldReturnFalseForPositionsUserAsync()
    {
      bool response = await _repository.ContainsUsersAsync(_position1.Id);
      SerializerAssert.AreEqual(false, response);
    }

    #endregion

    #region ExistNamesAndPositions

    [Test]
    public async Task ShouldReturnTrueForNameExistAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.DoesNameExistAsync("TestName1"));
    }

    [Test]
    public async Task ShouldReturnFalseForNameExistAsync()
    {
      SerializerAssert.AreEqual(false, await _repository.DoesNameExistAsync("Test"));
    }

    [Test]
    public async Task ShouldReturnTrueForExistAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.DoesExistAsync(_position1.Id));
    }

    [Test]
    public async Task ShouldReturnFalseForExistAsync()
    {
      SerializerAssert.AreEqual(false, await _repository.DoesExistAsync(Guid.NewGuid()));
    }

    #endregion

    #region EditPosition

    [Test]
    public async Task ShouldReturnPositionForEdit()
    {
      _mocker = new AutoMocker();
      _repository = _mocker.CreateInstance<PositionRepository>();

      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _creatorId);

      _mocker
        .Setup<IHttpContextAccessor, IDictionary<object, object>>(x => x.HttpContext.Items)
        .Returns(_items);

      DbPosition positionAfter = new DbPosition()
      {
        Id = _position1.Id,
        Name = "TestNameAfter",
        Description = "TestDescriptionAfter",
        IsActive = true,
        CreatedAtUtc = _position1.CreatedAtUtc,
        CreatedBy = _position1.CreatedBy,
      };

      JsonPatchDocument<DbPosition> patchPosition;

      patchPosition = new JsonPatchDocument<DbPosition>(new List<Operation<DbPosition>>
      {
        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.Name)}",
          "",
          $"{positionAfter.Name}"),

        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.Description)}",
          "",
          $"{positionAfter.Description}"),

        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.IsActive)}",
          "",
          $"{positionAfter.IsActive}"),
      }, new CamelCasePropertyNamesContractResolver());

      SerializerAssert.AreEqual(true, await _repository.EditAsync(_position1, patchPosition));

      positionAfter.ModifiedAtUtc = _position1.ModifiedAtUtc;
      positionAfter.ModifiedBy = _position1.ModifiedBy;
      SerializerAssert.AreEqual(positionAfter, _position1);
    }

    [Test]
    public async Task ShouldReturnFalseForEdit()
    {
      JsonPatchDocument<DbPosition> patchPosition;

      patchPosition = new JsonPatchDocument<DbPosition>(new List<Operation<DbPosition>>
      {
        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.Name)}",
          "",
          "Test"),

        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.Description)}",
          "",
          "Test"),

        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.IsActive)}",
          "",
          "false"),
      }, new CamelCasePropertyNamesContractResolver());

      SerializerAssert.AreEqual(false, await _repository.EditAsync(null, patchPosition));
    }

    #endregion

    [TearDown]
    public void CleanDb()
    {
      if (_provider.IsInMemory())
      {
        _provider.EnsureDeleted();
      }
    }
  }
}
