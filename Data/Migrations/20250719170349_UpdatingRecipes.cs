using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class UpdatingRecipes : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CraftedItemId",
            schema: "wow",
            table: "recipe",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "CraftedQuantity",
            schema: "wow",
            table: "recipe",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "Reagents",
            schema: "wow",
            table: "recipe",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateTable(
            name: "recipe_media",
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
                table.PrimaryKey("PK_recipe_media", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "recipe_media",
            schema: "wow");

        migrationBuilder.DropColumn(
            name: "CraftedItemId",
            schema: "wow",
            table: "recipe");

        migrationBuilder.DropColumn(
            name: "CraftedQuantity",
            schema: "wow",
            table: "recipe");

        migrationBuilder.DropColumn(
            name: "Reagents",
            schema: "wow",
            table: "recipe");
    }
}
