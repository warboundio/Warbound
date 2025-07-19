using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class ExtendingSlotCharacterLength : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Slot",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(63)",
            maxLength: 63,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(16)",
            oldMaxLength: 16);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Slot",
            schema: "wow",
            table: "item_appearance",
            type: "character varying(16)",
            maxLength: 16,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(63)",
            oldMaxLength: 63);
    }
}
