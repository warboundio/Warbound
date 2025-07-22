using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations;

/// <inheritdoc />
public partial class UpdatingAchievementProperties : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "CategoryName",
            schema: "wow",
            table: "achievement",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CriteriaIds",
            schema: "wow",
            table: "achievement",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "CriteriaTypes",
            schema: "wow",
            table: "achievement",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Description",
            schema: "wow",
            table: "achievement",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Icon",
            schema: "wow",
            table: "achievement",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "Points",
            schema: "wow",
            table: "achievement",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<string>(
            name: "RewardDescription",
            schema: "wow",
            table: "achievement",
            type: "character varying(2047)",
            maxLength: 2047,
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "RewardItemId",
            schema: "wow",
            table: "achievement",
            type: "integer",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "RewardItemName",
            schema: "wow",
            table: "achievement",
            type: "character varying(255)",
            maxLength: 255,
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CategoryName",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "CriteriaIds",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "CriteriaTypes",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "Description",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "Icon",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "Points",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "RewardDescription",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "RewardItemId",
            schema: "wow",
            table: "achievement");

        migrationBuilder.DropColumn(
            name: "RewardItemName",
            schema: "wow",
            table: "achievement");
    }
}
