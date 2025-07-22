using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingAddonObjects : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "g_loot_log_entry",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                NpcId = table.Column<int>(type: "integer", nullable: false),
                ItemId = table.Column<int>(type: "integer", nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false),
                ZoneId = table.Column<int>(type: "integer", nullable: false),
                X = table.Column<int>(type: "integer", nullable: false),
                Y = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_loot_log_entry", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "g_npc_kill_count",
            schema: "wow",
            columns: table => new
            {
                NpcId = table.Column<int>(type: "integer", nullable: false),
                Count = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_npc_kill_count", x => x.NpcId);
            });

        migrationBuilder.CreateTable(
            name: "g_pet_battle_location",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                MapId = table.Column<int>(type: "integer", nullable: false),
                X = table.Column<int>(type: "integer", nullable: false),
                Y = table.Column<int>(type: "integer", nullable: false),
                PetSpeciesId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_pet_battle_location", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "g_vendor",
            schema: "wow",
            columns: table => new
            {
                NpcId = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                Type = table.Column<string>(type: "character varying(31)", maxLength: 31, nullable: false),
                Faction = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                MapId = table.Column<int>(type: "integer", nullable: false),
                X = table.Column<int>(type: "integer", nullable: false),
                Y = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_vendor", x => x.NpcId);
            });

        migrationBuilder.CreateTable(
            name: "g_vendor_item",
            schema: "wow",
            columns: table => new
            {
                ItemId = table.Column<int>(type: "integer", nullable: false),
                VendorId = table.Column<int>(type: "integer", nullable: false),
                Quantity = table.Column<int>(type: "integer", nullable: false),
                Cost = table.Column<int>(type: "integer", nullable: false),
                CostType = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                CostId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_vendor_item", x => new { x.ItemId, x.VendorId });
            });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "g_loot_log_entry",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "g_npc_kill_count",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "g_pet_battle_location",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "g_vendor",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "g_vendor_item",
            schema: "wow");
    }
}
