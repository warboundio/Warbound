using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ETL.Migrations;

/// <inheritdoc />
public partial class AddingItemAppearanceObject : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "wow");

        migrationBuilder.CreateTable(
            name: "item_appearance",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Slot = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                Class = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                Subclass = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                DisplayInfoId = table.Column<int>(type: "integer", nullable: false),
                ItemIds = table.Column<string>(type: "character varying(511)", maxLength: 511, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_item_appearance", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "item_appearance",
            schema: "wow");
    }
}
