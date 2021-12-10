using System;
using LT.DigitalOffice.PositionService.Models.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.PositionService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(PositionServiceDbContext))]
  [Migration("20211111124300_AddUsersRatesTable")]
  public class AddUsersRatesTable : Migration
  {
    private void CreateUsersRatesTable(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.CreateTable(
        name: "UsersRates",
        columns: table => new
        {
          Id = table.Column<Guid>(nullable: false),
          UserId = table.Column<Guid>(nullable: false),
          Rate = table.Column<double>(nullable: false),
          IsActive = table.Column<bool>(nullable: false),
          CreatedBy = table.Column<Guid>(nullable: false),
          CreatedAtUtc = table.Column<DateTime>(nullable: false),
          ModifiedBy = table.Column<Guid>(nullable: true),
          ModifiedAtUtc = table.Column<DateTime>(nullable: true)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_UsersRates", u => u.Id);
        });
    }

    protected override void Up(MigrationBuilder migrationBuilder)
    {
      CreateUsersRatesTable(migrationBuilder);

      migrationBuilder.Sql(@"INSERT INTO UsersRates (Id, UserId, Rate, IsActive, CreatedBy, CreatedAtUtc, ModifiedBy, ModifiedAtUtc)
                            (SELECT newid() as Id, UserId, Rate, IsActive, CreatedBy, CreatedAtUtc, Null as ModifiedBy, Null as ModifiedAtUtc
                            FROM[PositionDB].[dbo].[PositionUsers])");

      migrationBuilder.DropColumn(
        name: "Rate",
        table: DbPositionUser.TableName);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable("DbUserRate");

      migrationBuilder.AddColumn<double>(
        name: "Rate",
        table: DbPositionUser.TableName,
        defaultValue: 1);
    }
  }
}
