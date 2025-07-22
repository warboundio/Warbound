using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class UpdatingQuestProperties : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "QuestTypeId",
            schema: "wow",
            table: "quest",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "RewardItems",
            schema: "wow",
            table: "quest",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "QuestTypeId",
            schema: "wow",
            table: "quest");

        migrationBuilder.DropColumn(
            name: "RewardItems",
            schema: "wow",
            table: "quest");
    }
}
