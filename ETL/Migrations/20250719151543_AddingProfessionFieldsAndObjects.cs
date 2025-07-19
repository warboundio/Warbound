using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ETL.Migrations;

/// <inheritdoc />
public partial class AddingProfessionFieldsAndObjects : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "SkillTiers",
            schema: "wow",
            table: "profession",
            type: "character varying(1027)",
            maxLength: 1027,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Type",
            schema: "wow",
            table: "profession",
            type: "character varying(127)",
            maxLength: 127,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "profession_media",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                URL = table.Column<string>(type: "character varying(1023)", maxLength: 1023, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_profession_media", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "profession_media",
            schema: "wow");

        migrationBuilder.DropColumn(
            name: "SkillTiers",
            schema: "wow",
            table: "profession");

        migrationBuilder.DropColumn(
            name: "Type",
            schema: "wow",
            table: "profession");
    }
}
