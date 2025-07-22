using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingQuestIndexes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "journal_instance_media",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                URL = table.Column<string>(type: "character varying(1023)", maxLength: 1023, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_journal_instance_media", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "quest_area",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_quest_area", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "quest_category",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_quest_category", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "quest_type",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false),
                Name = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_quest_type", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "journal_instance_media",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "quest_area",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "quest_category",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "quest_type",
            schema: "wow");
    }
}
