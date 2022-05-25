using System;
using LT.DigitalOffice.PositionService.Models.Db;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.PositionService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(PositionServiceDbContext))]
  [Migration("20220522164300_InitialMigration")]
  public class InitialMigration : Migration
  {
    private void CreatePositionTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        DbPosition.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          Name = table.Column<string>(nullable: false, maxLength: 80),
          Description = table.Column<string>(nullable: true),
          IsActive = table.Column<bool>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_Positions", p => p.Id);
          table.UniqueConstraint("UK_Positions_Name", p => p.Name);
        });
    }

    private void CreatePositionUsersTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        DbPositionUser.TableName,
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          PositionId = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          Rate = table.Column<double>(nullable: false),
          IsActive = table.Column<bool>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          PeriodEnd = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"),
          PeriodStart = table.Column<DateTime>(type: "datetime2", nullable: false)
            .Annotation("SqlServer:IsTemporal", true)
            .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
            .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart")
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_PositionUsers", p => p.Id);
        })
        .Annotation("SqlServer:IsTemporal", true)
        .Annotation("SqlServer:TemporalHistoryTableName", DbPositionUser.HistoryTableName)
        .Annotation("SqlServer:TemporalPeriodEndColumnName", "PeriodEnd")
        .Annotation("SqlServer:TemporalPeriodStartColumnName", "PeriodStart"); ;
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      CreatePositionTable(migrationBuilder);
      CreatePositionUsersTable(migrationBuilder);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable(DbPosition.TableName);
      migrationBuilder.DropTable(DbPositionUser.TableName);
    }
  }
}
