using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Data;
using LT.DigitalOffice.PositionService.Data.Provider;
using LT.DigitalOffice.PositionService.Data.Provider.MsSql.Ef;
using LT.DigitalOffice.PositionService.Models.Db;
using LT.DigitalOffice.UnitTestKernel;
using Microsoft.EntityFrameworkCore;
using Moq.AutoMock;
using NUnit.Framework;

namespace PositionService.Data.UnitTests
{
  public class PositionUserRepositoryTests
  {
    private IDataProvider _provider;
    private PositionUserRepository _repository;
    private DbContextOptions<PositionServiceDbContext> _dbContext;

    private DbPositionUser _positionUser1;
    private DbPositionUser _positionUser2;

    private DbPosition _position1;
    private DbPosition _position2;

    [SetUp]
    public void SetUp()
    {
      CreatePositions();

      CreateMemoryDb();

      SavePositions();
    }

    private void CreatePositions()
    {
      _positionUser1 = new DbPositionUser()
      {
        Id = Guid.NewGuid(),
        PositionId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        IsActive = true,
        CreatedBy = Guid.NewGuid()
      };

      _positionUser2 = new DbPositionUser()
      {
        Id = Guid.NewGuid(),
        PositionId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        IsActive = true,
        CreatedBy = Guid.NewGuid()
      };

      _position1 = new DbPosition()
      {
        Id = _positionUser1.PositionId,
        Name = "TestName1",
        Description = "TestDescription1",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = Guid.NewGuid(),
      };

      _position2 = new DbPosition()
      {
        Id = _positionUser2.PositionId,
        Name = "TestName2",
        Description = "TestDescription2",
        IsActive = true,
        CreatedAtUtc = DateTime.UtcNow,
        CreatedBy = Guid.NewGuid(),
      };
    }

    public void CreateMemoryDb()
    {
      _dbContext = new DbContextOptionsBuilder<PositionServiceDbContext>()
        .UseInMemoryDatabase(databaseName: "PositionServiceTests")
        .Options;

      _provider = new PositionServiceDbContext(_dbContext);
      _repository = new PositionUserRepository(_provider);
    }

    public void SavePositions()
    {
      _provider.PositionsUsers.AddRange(_positionUser1);
      _provider.PositionsUsers.AddRange(_positionUser2);
      _provider.Positions.AddRange(_position1);
      _provider.Positions.AddRange(_position2);
      _provider.Save();
    }

    #region AddPositionUser

    [Test]
    public async Task ShouldAddPositionUserAsync()
    {
      DbPositionUser position = new DbPositionUser
      {
        Id = Guid.NewGuid(),
        PositionId = Guid.NewGuid(),
        UserId = Guid.NewGuid(),
        IsActive = true,
        CreatedBy = Guid.NewGuid()
      };

      SerializerAssert.AreEqual(position.Id, await _repository.CreateAsync(position));
    }

    [Test]
    public async Task ShouldReturnNullIfRequestIsNullAsync()
    {
      SerializerAssert.AreEqual(null, await _repository.CreateAsync(null));
    }

    #endregion

    #region GetPositionUser

    [Test]
    public async Task ShouldReturnPositionForIdAsync()
    {
      SerializerAssert.AreEqual(_positionUser1, await _repository.GetAsync(_positionUser1.UserId));
    }

    [Test]
    public async Task ShouldReturnNullPositionForIdAsync()
    {
      SerializerAssert.AreEqual(null, await _repository.GetAsync(Guid.NewGuid()));
    }

    [Test]
    public async Task ShouldReturnPositionsForIdsAsync()
    {
      List<Guid> ids = new List<Guid>() { _positionUser1.UserId, _positionUser2.UserId };

      List<DbPositionUser> expectedResponse = new List<DbPositionUser>() { _positionUser1, _positionUser2 };

      SerializerAssert.AreEqual(expectedResponse, await _repository.GetAsync(ids));
    }

    #endregion

    #region EditPosition

    [Test]
    public async Task ShouldReturnPositionForEdit()
    {
      SerializerAssert.AreEqual(_positionUser1.PositionId, await _repository.EditAsync(_positionUser1.UserId, _positionUser1.PositionId));
    }

    [Test]
    public async Task ShouldReturnNullIfRequestIsNull()
    {
      SerializerAssert.AreEqual(null, await _repository.EditAsync(Guid.NewGuid(), _positionUser1.PositionId));
    }

    #endregion

    #region RemovePosition

    [Test]
    public async Task ShouldReturnPositionForRemove()
    {
      SerializerAssert.AreEqual(_positionUser1.PositionId, await _repository.RemoveAsync(_positionUser1.UserId, Guid.NewGuid()));
    }

    [Test]
    public async Task ShouldReturnNullForRemove()
    {
      SerializerAssert.AreEqual(null, await _repository.RemoveAsync(Guid.NewGuid(), Guid.NewGuid()));
    }

    #endregion

    #region ExistPosition

    [TestCase(true)]
    [TestCase(false)]
    public async Task ShouldReturnIsPositionExists(bool result)
    {
      if (result)
      {
        SerializerAssert.AreEqual(result, await _repository.DoesExistAsync(_positionUser1.UserId));
      }
      else
      {
        SerializerAssert.AreEqual(result, await _repository.DoesExistAsync(Guid.NewGuid()));
      }
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
