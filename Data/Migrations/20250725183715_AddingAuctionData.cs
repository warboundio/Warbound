using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingAuctionData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "auction",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ItemId = table.Column<int>(type: "integer", nullable: false),
                CreatedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsCommodity = table.Column<bool>(type: "boolean", nullable: false),
                Price = table.Column<long>(type: "bigint", nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_auction", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "auction",
            schema: "wow");
    }
}
