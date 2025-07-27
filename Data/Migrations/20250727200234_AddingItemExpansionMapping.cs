using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingItemExpansionMapping : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "item_expansion",
            schema: "wow",
            columns: table => new
            {
                ItemId = table.Column<int>(type: "integer", nullable: false),
                ExpansionId = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_item_expansion", x => x.ItemId);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "item_expansion",
            schema: "wow");
    }
}