using System.Reflection;
using System.Threading.Tasks;
using LT.DigitalOffice.PositionService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.PositionService.Data.Provider.MsSql.Ef
{
  /// <summary>
  /// A class that defines the tables and its properties in the database of PositionService.
  /// </summary>
  public class PositionServiceDbContext : DbContext, IDataProvider
  {
    public PositionServiceDbContext(DbContextOptions<PositionServiceDbContext> options)
        : base(options)
    {
    }

    public DbSet<DbPosition> Positions { get; set; }
    public DbSet<DbPositionUser> PositionsUsers { get; set; }

    // Fluent API is written here.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfigurationsFromAssembly(Assembly.Load("LT.DigitalOffice.PositionService.Models.Db"));
    }

    public object MakeEntityDetached(object obj)
    {
      Entry(obj).State = EntityState.Detached;

      return Entry(obj).State;
    }

    public void Save()
    {
      SaveChanges();
    }

    public void EnsureDeleted()
    {
      Database.EnsureDeleted();
    }

    public bool IsInMemory()
    {
      return Database.IsInMemory();
    }

    public async Task SaveAsync()
    {
      await SaveChangesAsync();
    }
  }
}
