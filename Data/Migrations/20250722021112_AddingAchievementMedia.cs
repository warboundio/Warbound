using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingAchievementMedia : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "DungeonIds",
            schema: "wow",
            table: "journal_expansion",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "RaidIds",
            schema: "wow",
            table: "journal_expansion",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "achievement_media",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                URL = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_achievement_media", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "achievement_media",
            schema: "wow");

        migrationBuilder.DropColumn(
            name: "DungeonIds",
            schema: "wow",
            table: "journal_expansion");

        migrationBuilder.DropColumn(
            name: "RaidIds",
            schema: "wow",
            table: "journal_expansion");
    }
}
