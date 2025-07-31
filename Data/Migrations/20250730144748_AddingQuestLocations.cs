using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingQuestLocations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "g_quest_location",
            schema: "wow",
            columns: table => new
            {
                QuestId = table.Column<int>(type: "integer", nullable: false),
                IsStart = table.Column<bool>(type: "boolean", nullable: false),
                NpcName = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                MapId = table.Column<int>(type: "integer", nullable: false),
                X = table.Column<int>(type: "integer", nullable: false),
                Y = table.Column<int>(type: "integer", nullable: false),
                NpcId = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_g_quest_location", x => new { x.QuestId, x.IsStart });
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "g_quest_location",
            schema: "wow");
    }
}
