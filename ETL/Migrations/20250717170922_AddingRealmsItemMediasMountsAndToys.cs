using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ETL.Migrations;

/// <inheritdoc />
public partial class AddingRealmsItemMediasMountsAndToys : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "item_media",
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
                table.PrimaryKey("PK_item_media", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "mount",
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
                table.PrimaryKey("PK_mount", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "realm",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Slug = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Category = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Locale = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Timezone = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Type = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                IsTournament = table.Column<bool>(type: "boolean", nullable: false),
                Region = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_realm", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "toy",
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
                table.PrimaryKey("PK_toy", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "item_media",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "mount",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "realm",
            schema: "wow");

        migrationBuilder.DropTable(
            name: "toy",
            schema: "wow");
    }
}
