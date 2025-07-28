using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingObjectExpansionMappings : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "item_expansion",
            schema: "wow");

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

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "object_expansion",
            schema: "wow");

        migrationBuilder.CreateTable(
            name: "item_expansion",
            schema: "wow",
            columns: table => new
            {
                ItemId = table.Column<int>(type: "integer", nullable: false),
                ExpansionId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_item_expansion", x => x.ItemId);
            });
    }
}
