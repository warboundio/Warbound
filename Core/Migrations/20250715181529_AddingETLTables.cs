using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Core.Migrations;

/// <inheritdoc />
public partial class AddingETLTables : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "application");

        migrationBuilder.CreateTable(
            name: "etl_jobs",
            schema: "application",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(1023)", maxLength: 1023, nullable: false),
                CRONSchedule = table.Column<string>(type: "character varying(63)", maxLength: 63, nullable: false),
                LastSuccessAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LockOwner = table.Column<string>(type: "character varying(127)", maxLength: 127, nullable: true),
                LockAcquiredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                LockTimeoutMinutes = table.Column<int>(type: "integer", nullable: false),
                WasLastRunSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                LastDurationMilliseconds = table.Column<int>(type: "integer", nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_etl_jobs", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_etl_jobs_Name",
            schema: "application",
            table: "etl_jobs",
            column: "Name",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "etl_jobs",
            schema: "application");
    }
}
