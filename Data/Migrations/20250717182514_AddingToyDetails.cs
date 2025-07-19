using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingToyDetails : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "MediaId",
            schema: "wow",
            table: "toy",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "SourceType",
            schema: "wow",
            table: "toy",
            type: "character varying(31)",
            maxLength: 31,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "MediaId",
            schema: "wow",
            table: "toy");

        migrationBuilder.DropColumn(
            name: "SourceType",
            schema: "wow",
            table: "toy");
    }
}
