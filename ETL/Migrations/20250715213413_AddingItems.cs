using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ETL.Migrations;

/// <inheritdoc />
public partial class AddingItems : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "item",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                QualityType = table.Column<int>(type: "integer", nullable: false),
                Level = table.Column<int>(type: "integer", nullable: false),
                RequiredLevel = table.Column<int>(type: "integer", nullable: false),
                ClassType = table.Column<int>(type: "integer", nullable: false),
                SubclassType = table.Column<int>(type: "integer", nullable: false),
                InventoryType = table.Column<int>(type: "integer", nullable: false),
                PurchasePrice = table.Column<int>(type: "integer", nullable: false),
                SellPrice = table.Column<int>(type: "integer", nullable: false),
                MaxCount = table.Column<int>(type: "integer", nullable: false),
                IsEquippable = table.Column<bool>(type: "boolean", nullable: false),
                IsStackable = table.Column<bool>(type: "boolean", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_item", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "item",
            schema: "wow");
    }
}
