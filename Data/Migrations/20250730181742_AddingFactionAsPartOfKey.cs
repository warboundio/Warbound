using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class AddingFactionAsPartOfKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_g_vendor_item",
            schema: "wow",
            table: "g_vendor_item");

        migrationBuilder.DropPrimaryKey(
            name: "PK_g_quest_location",
            schema: "wow",
            table: "g_quest_location");

        migrationBuilder.AddPrimaryKey(
            name: "PK_g_vendor_item",
            schema: "wow",
            table: "g_vendor_item",
            columns: ["ItemId", "VendorId", "FactionId"]);

        migrationBuilder.AddPrimaryKey(
            name: "PK_g_quest_location",
            schema: "wow",
            table: "g_quest_location",
            columns: ["QuestId", "IsStart", "FactionId"]);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_g_vendor_item",
            schema: "wow",
            table: "g_vendor_item");

        migrationBuilder.DropPrimaryKey(
            name: "PK_g_quest_location",
            schema: "wow",
            table: "g_quest_location");

        migrationBuilder.AddPrimaryKey(
            name: "PK_g_vendor_item",
            schema: "wow",
            table: "g_vendor_item",
            columns: ["ItemId", "VendorId"]);

        migrationBuilder.AddPrimaryKey(
            name: "PK_g_quest_location",
            schema: "wow",
            table: "g_quest_location",
            columns: ["QuestId", "IsStart"]);
    }
}
