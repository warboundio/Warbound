using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ETL.Migrations;

/// <inheritdoc />
public partial class AddingPetData : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "pet",
            schema: "wow",
            columns: table => new
            {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                Name = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                BattlePetType = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                IsCapturable = table.Column<bool>(type: "boolean", nullable: false),
                IsTradable = table.Column<bool>(type: "boolean", nullable: false),
                IsBattlePet = table.Column<bool>(type: "boolean", nullable: false),
                IsAllianceOnly = table.Column<bool>(type: "boolean", nullable: false),
                IsHordeOnly = table.Column<bool>(type: "boolean", nullable: false),
                SourceType = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: false),
                Icon = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                LastUpdatedUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_pet", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "pet",
            schema: "wow");
    }
}
