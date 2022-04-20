using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
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

    private DbPosition _position;
    private Guid _creatorId = Guid.NewGuid();

    private AutoMocker _mocker;
    private Mock<IHttpContextAccessor> _accessorMock;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
      _position = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName",
        Description = "TestDescription",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      _dbContext = new DbContextOptionsBuilder<PositionServiceDbContext>()
                 .UseInMemoryDatabase(databaseName: "PositionServiceTes")
                 .Options;
    }

    [SetUp]
    public void SetUp()
    {
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
    public async Task AddPositionTest()
    {
      SerializerAssert.AreEqual(_position.Id, await _repository.CreateAsync(_position));
    }

    [Test]
    public async Task AddNullPositionTest()
    {
      SerializerAssert.AreEqual(null, await _repository.CreateAsync(null));
    }

    [Test]
    public async Task GetPositionTest()
    {
      SerializerAssert.AreEqual(_position, await _repository.GetAsync(_position.Id));
    }

    [Test]
    public async Task GetNullPositionTest()
    {
      SerializerAssert.AreEqual(null, await _repository.GetAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task FindPositionTest() //?
    {
      DbPosition position = new DbPosition()
      {
        Id = Guid.NewGuid(),
        Name = "TestName2",
        Description = "TestDescription2",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = _creatorId,
      };

      await _repository.CreateAsync(position);
      List<DbPosition> positions = new List<DbPosition>() { _position, position };
      FindPositionsFilter filter = new FindPositionsFilter()
      {
        SkipCount = 0,
        TakeCount = 10,
      };

      (List<DbPosition>, int) expectedResponse = (positions, 2);
      var response = await _repository.FindAsync(filter);

      SerializerAssert.AreEqual(expectedResponse, response);
    }
  }
}
