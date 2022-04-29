using System;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LT.DigitalOffice.PositionService.Models.Db
{
  public class DbPositionUser
  {
    public const string TableName = "PositionUsers";

    public Guid Id { get; set; }
    public Guid PositionId { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; }
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public Guid? ModifiedBy { get; set; }
    public DateTime? ModifiedAtUtc { get; set; }

    [JsonIgnore]
    public DbPosition Position { get; set; }
  }

  public class DbPositionUserConfiguration : IEntityTypeConfiguration<DbPositionUser>
  {
    public void Configure(EntityTypeBuilder<DbPositionUser> builder)
    {
      builder
        .ToTable(DbPositionUser.TableName);

      builder
        .HasKey(p => p.Id);

      builder
        .HasOne(pu => pu.Position)
        .WithMany(p => p.Users);
    }
  }
}
