using LT.DigitalOffice.Kernel.Attributes;
using LT.DigitalOffice.Kernel.Database;
using LT.DigitalOffice.Kernel.Enums;
using LT.DigitalOffice.PositionService.Models.Db;
using Microsoft.EntityFrameworkCore;

namespace LT.DigitalOffice.PositionService.Data.Provider
{
  [AutoInject(InjectType.Scoped)]
  public interface IDataProvider : IBaseDataProvider
  {
    DbSet<DbPosition> Positions { get; set; }
    DbSet<DbPositionUser> PositionsUsers { get; set; }
  }
}
