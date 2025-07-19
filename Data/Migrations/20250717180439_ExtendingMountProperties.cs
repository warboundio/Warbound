using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class ExtendingMountProperties : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "CreatureDisplayId",
            schema: "wow",
            table: "mount",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "SourceType",
            schema: "wow",
            table: "mount",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatureDisplayId",
            schema: "wow",
            table: "mount");

        migrationBuilder.DropColumn(
            name: "SourceType",
            schema: "wow",
            table: "mount");
    }
}
