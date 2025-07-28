using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingLootItemSummariesAndLocationSummaries : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "g_loot_log_entry",
            schema: "wow");

        migrationBuilder.CreateTable(
            name: "g_loot_item_summary",
            schema: "wow",
            columns: table => new
            {
                NpcId = table.Column<int>(type: "integer", nullable: false),
                ItemId = table.Column<int>(type: "integer", nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_loot_item_summary", x => new { x.NpcId, x.ItemId });
            });

        migrationBuilder.CreateTable(
            name: "g_loot_location_entry",
            schema: "wow",
            columns: table => new
            {
                NpcId = table.Column<int>(type: "integer", nullable: false),
                X = table.Column<int>(type: "integer", nullable: false),
                Y = table.Column<int>(type: "integer", nullable: false),
                ZoneId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_loot_location_entry", x => new { x.NpcId, x.X, x.Y, x.ZoneId });
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "g_loot_item_summary",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "g_loot_location_entry",
            schema: "wow");

        migrationBuilder.CreateTable(
            name: "g_loot_log_entry",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                ItemId = table.Column<int>(type: "integer", nullable: false),
                NpcId = table.Column<int>(type: "integer", nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false),
                X = table.Column<int>(type: "integer", nullable: false),
                Y = table.Column<int>(type: "integer", nullable: false),
                ZoneId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_loot_log_entry", x => x.Id);
            });
    }
}
