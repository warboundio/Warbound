using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

/// <inheritdoc />
public partial class AddGitHubIssuesTable : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "github_issues",
            schema: "application",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                IssueId = table.Column<int>(type: "integer", nullable: false),
                CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Name = table.Column<string>(type: "character varying(1023)", maxLength: 1023, nullable: false),
                WaitingForYou = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_github_issues", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "github_issues",
            schema: "application");
    }
}