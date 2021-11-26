using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.PositionService.Models.Db
{
  public class DbUserRate
  {
    public const string TableName = "UsersRates";

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public double Rate { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }
  }

  public class DbUserRateConfiguration : IEntityTypeConfiguration<DbUserRate>
  {
    public void Configure(EntityTypeBuilder<DbUserRate> builder)
    {
      builder
        .ToTable(DbUserRate.TableName);

      builder
        .HasKey(u => u.Id);
    }
  }
}
