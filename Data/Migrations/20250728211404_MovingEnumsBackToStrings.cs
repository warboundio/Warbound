using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class MovingEnumsBackToStrings : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "SubclassType",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "SlotType",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "ClassType",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "SubclassType",
            schema: "wow",
            table: "item",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "InventoryType",
            schema: "wow",
            table: "item",
            type: "character varying(127)",
            maxLength: 127,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");

        migrationBuilder.AlterColumn<string>(
            name: "ClassType",
            schema: "wow",
            table: "item",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            oldClrType: typeof(int),
            oldType: "integer");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<int>(
            name: "SubclassType",
            schema: "wow",
            table: "item_appearance",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(63)",
            oldMaxLength: 63);

        migrationBuilder.AlterColumn<int>(
            name: "SlotType",
            schema: "wow",
            table: "item_appearance",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(63)",
            oldMaxLength: 63);

        migrationBuilder.AlterColumn<int>(
            name: "ClassType",
            schema: "wow",
            table: "item_appearance",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(63)",
            oldMaxLength: 63);

        migrationBuilder.AlterColumn<int>(
            name: "SubclassType",
            schema: "wow",
            table: "item",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(63)",
            oldMaxLength: 63);

        migrationBuilder.AlterColumn<int>(
            name: "InventoryType",
            schema: "wow",
            table: "item",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(127)",
            oldMaxLength: 127);

        migrationBuilder.AlterColumn<int>(
            name: "ClassType",
            schema: "wow",
            table: "item",
            type: "integer",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(63)",
            oldMaxLength: 63);
    }
}
