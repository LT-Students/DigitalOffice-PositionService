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
    private DbPosition _position3;
    private Guid _creatorId = Guid.NewGuid();

    private DbPositionUser _user;

    private AutoMocker _mocker;
    private IHttpContextAccessor _contextAccessor;
  //  private Mock<IGetPositionsRequest> _getRequest;

    [SetUp]
    public void SetUp()
    {
      _position1 = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName",
        Description = "TestDescription",
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

      _position3 = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName3",
        Description = "TestDescription3",
        IsActive = false,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      _user = new DbPositionUser()
      {
        Id = Guid.NewGuid(),
        PositionId = _position3.Id,
        UserId = Guid.NewGuid(),
        IsActive = true,
        CreatedBy = _creatorId,
        CreatedAtUtc = DateTime.UtcNow
      };

      CreateMemoryDb();

      _provider.Positions.AddRange(_position1);
      _provider.Positions.AddRange(_position2);
      _provider.Positions.AddRange(_position3);
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

    [TearDown]
    public void CleanDb()
    {
      if (_provider.IsInMemory())
      {
        _provider.EnsureDeleted();
      }
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
    public async Task GetPositionsforRequest()
    {
      List<DbPosition> positions = new List<DbPosition>() { _position1, _position2 };
      List<Guid> ids = new List<Guid>() { _position1.Id, _position2.Id };

      // IGetPositionsRequest.CreateObj(usersIds: ids);

      //_getRequest
      //  .Setup(x => x.UsersIds)
      //  .Returns(ids);  

      // SerializerAssert.AreEqual(positions, await _repository.GetAsync(IGetPositionsRequest.CreateObj(usersIds: ids)));
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

    #endregion

    #region FindPositions

    [Test]
    public async Task ShouldReturnListOfActivePositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition>() { _position1, _position2 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 2);
      var response = await _repository.FindAsync(filter);

      SerializerAssert.AreEqual(expectedResponse, response);
    }


    [Test]
    public async Task ShouldReturnListOfAllPositionsAsync()
    {
      List<DbPosition> positions = new List<DbPosition> { _position1, _position2, _position3 };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        IncludeDeactivated = true,
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 3);
      var response = await _repository.FindAsync(filter);

      SerializerAssert.AreEqual(expectedResponse, response);
    }

    #endregion

    #region ContainsUsers

    [Test]
    public async Task ShouldReturnTrueForPositionsUserAsync()
    {
      SerializerAssert.AreEqual(true, await _repository.ContainsUsersAsync(_position3.Id));
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
      SerializerAssert.AreEqual(true, await _repository.DoesNameExistAsync("TestName"));
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

    [Test]
    public async Task ShouldSomethiiing()
    {
     // _contextAccessor = _mocker.CreateInstance<HttpContextAccessor>();
      
      //_mocker
      //  .Setup<IHttpContextAccessor, Guid>(x => x.HttpContext)
      //  .Returns(_creatorId);

      DbPosition position3After = new DbPosition()
      {
        Id = _position3.Id,
        Name = "TestName3After",
        Description = "TestDescription3After",
        IsActive = true,
        CreatedAtUtc = _position3.CreatedAtUtc,
        CreatedBy = _position3.CreatedBy,
      };

      JsonPatchDocument<DbPosition> patchPosition;

      patchPosition = new JsonPatchDocument<DbPosition>(new List<Operation<DbPosition>>
      {
        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.Name)}",
          "",
          $"{position3After.Name}"),

        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.Description)}",
          "",
          $"{position3After.Description}"),

        new Operation<DbPosition>(
          "replace",
          $"/{nameof(DbPosition.IsActive)}",
          "",
          $"{position3After.IsActive}"),
      }, new CamelCasePropertyNamesContractResolver());

     // SerializerAssert.AreEqual(true, await _repository.EditAsync(_position3, patchPosition));

     // var patchedPosition = _provider.Positions.FirstOrDefaultAsync(p => p.Id == _position3.Id);
     // position3After.ModifiedAtUtc = _position3.ModifiedAtUtc;
      //position3After.ModifiedBy = _position3.ModifiedBy;
     // SerializerAssert.AreEqual(position3After, _position3);

    }


    //istrue,isfalse,isnull

  }
}
