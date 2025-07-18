using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ETL.Migrations;

/// <inheritdoc />
public partial class UpdatingStringsToFitEnums : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Class",
            schema: "wow",
            table: "item_appearance");

        migrationBuilder.DropColumn(
            name: "Slot",
            schema: "wow",
            table: "item_appearance");

        migrationBuilder.DropColumn(
            name: "Subclass",
            schema: "wow",
            table: "item_appearance");

        migrationBuilder.AddColumn<int>(
            name: "ClassType",
            schema: "wow",
            table: "item_appearance",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "SlotType",
            schema: "wow",
            table: "item_appearance",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<int>(
            name: "SubclassType",
            schema: "wow",
            table: "item_appearance",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ClassType",
            schema: "wow",
            table: "item_appearance");

        migrationBuilder.DropColumn(
            name: "SlotType",
            schema: "wow",
            table: "item_appearance");

        migrationBuilder.DropColumn(
            name: "SubclassType",
            schema: "wow",
            table: "item_appearance");

        migrationBuilder.AddColumn<string>(
            name: "Class",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Slot",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Subclass",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            defaultValue: "");
    }
}
