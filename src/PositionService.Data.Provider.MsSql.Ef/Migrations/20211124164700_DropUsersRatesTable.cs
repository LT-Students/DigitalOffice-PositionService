﻿using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LT.DigitalOffice.PositionService.Data.Provider.MsSql.Ef.Migrations
{
  [DbContext(typeof(PositionServiceDbContext))]
  [Migration("20211124164700_DropUsersRatesTable")]
  public class DropUsersRatesTable : Migration
  {
    protected override void Up(MigrationBuilder migrationBuilder)
    {
      migrationBuilder.DropTable("UsersRates");
    }
  }
}