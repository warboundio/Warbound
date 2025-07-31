using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingFactionsToGameData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "object_expansion",
            schema: "wow");

        migrationBuilder.AddColumn<int>(
            name: "FactionId",
            schema: "wow",
            table: "g_vendor_item",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "FactionId",
            schema: "wow",
            table: "g_quest_location",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "FactionId",
            schema: "wow",
            table: "g_vendor_item");

        migrationBuilder.DropColumn(
            name: "FactionId",
            schema: "wow",
            table: "g_quest_location");

        migrationBuilder.CreateTable(
            name: "object_expansion",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                CollectionType = table.Column<char>(type: "character(1)", nullable: false),
                ExpansionId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_object_expansion", x => new { x.Id, x.CollectionType });
            });
    }
}
