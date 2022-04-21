using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using LT.DigitalOffice.Models.Broker.Requests.Position;
using LT.DigitalOffice.PositionService.Data.Interfaces;
using LT.DigitalOffice.PositionService.Data.Provider;
using LT.DigitalOffice.PositionService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.PositionService.Models.Dto.Requests.Position.Filters;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.AutoMock;
using NUnit.Framework;

namespace LT.DigitalOffice.PositionService.Data.UnitTests
{
  public class PositionRepositoryTests
  {
    private IDataProvider _provider;
    private IPositionRepository _repository;
    private DbContextOptions<PositionServiceDbContext> _dbContext;

    private List<DbPosition> _positions;
    private DbPosition _position1;
    private DbPosition _position2;
    private DbPosition _position3;
    private Guid _creatorId = Guid.NewGuid();

    private DbPositionUser _user;

    private AutoMocker _mocker;
    private Mock<IHttpContextAccessor> _accessorMock;

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

      _positions = new List<DbPosition>() { _position1, _position2};

      CreateMemoryDb();

      _provider.Positions.AddRange(_position1);
      _provider.Positions.AddRange(_position2);
      _provider.Positions.AddRange(_position3);
      _provider.PositionsUsers.AddRange(_user);
      _provider.Save();
    }

    public void CreateMemoryDb()
    {
      _dbContext = new DbContextOptionsBuilder<PositionServiceDbContext>()
                 .UseInMemoryDatabase(databaseName: "PositionServiceTes")
                 .Options;

      _accessorMock = new();
      IDictionary<object, object> _items = new Dictionary<object, object>();
      _items.Add("UserId", _creatorId);

      _accessorMock
       .Setup(x => x.HttpContext.Items)
       .Returns(_items);

      _provider = new PositionServiceDbContext(_dbContext);
      _repository = new PositionRepository(_provider, _accessorMock.Object);
    }

    //[TearDown]
    //public void CleanDb()
    //{
    //  if (_provider.IsInMemory())
    //  {
    //    _provider.EnsureDeleted();
    //  }
    //}

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
    public async Task ShouldReturnListOfPositionsAsync() //?
    {
      List<DbPosition> positions = new List<DbPosition>() { _position1, _position2, _position3};
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 3);
      var response = await _repository.FindAsync(filter);

      SerializerAssert.AreEqual(expectedResponse, response);
    }

    //[Test]
    //public async Task GetPositionsforRequest()
    //{
    //  IGetPositionsRequest request;

    //  List<Guid> ids = new List<Guid>() { _position1.Id, _position2.Id };

    //  SerializerAssert.AreEqual(_positions, await _repository.GetAsync());
    //}

    [Test]
    public async Task ShouldReturnListPositionsForIdsAsync()
    {
      List<Guid> ids = new List<Guid>() { _position1.Id, _position2.Id };

      SerializerAssert.AreEqual(_positions, await _repository.GetAsync(ids));
    }

    [Test]
    public async Task ShouldReturnNullPositionsForIdsAsync()
    {
      List<Guid> ids = new List<Guid>() {};
      List<DbPosition> positions = new List<DbPosition>() { };

      SerializerAssert.AreEqual(positions, await _repository.GetAsync(ids));
    }

    public async Task ShouldReturnTrueForPositionsUserAsync() //!!
    {
      SerializerAssert.AreEqual(true, await _repository.ContainsUsersAsync(_position3.Id));
    }

    public async Task ShouldReturnFalseForPositionsUserAsync() //!!
    {
      SerializerAssert.AreEqual(false, await _repository.ContainsUsersAsync(_position1.Id));
    }

    public async Task ShouldReturnTrueForNameExistAsync() //
    {
      SerializerAssert.AreEqual(true, await _repository.DoesNameExistAsync("TestName"));
    }

    public async Task ShouldReturnFalseForNameExistAsync() //
    {
      SerializerAssert.AreEqual(false, await _repository.DoesNameExistAsync("Test"));
    }

    public async Task ShouldReturnTrueForExistAsync() // null
    {
      SerializerAssert.AreEqual(true, await _repository.DoesExistAsync(_position1.Id));
    }

    public async Task ShouldReturnFalseForExistAsync() // null
    {
      SerializerAssert.AreEqual(false, await _repository.DoesExistAsync(Guid.NewGuid()));
    }
  }
}
