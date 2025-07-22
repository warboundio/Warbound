using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingBlizObjs : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "achievement",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_achievement", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "journal_encounter",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Items = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: false),
                InstanceName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                InstanceId = table.Column<int>(type: "integer", nullable: false),
                CategoryType = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                ModesTypes = table.Column<string>(type: "character varying(2047)", maxLength: 2047, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_journal_encounter", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "journal_expansion",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_journal_expansion", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "achievement",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "journal_encounter",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "journal_expansion",
            schema: "wow");
    }
}
