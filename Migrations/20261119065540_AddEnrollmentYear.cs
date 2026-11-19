using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMVC1.Migrations
{
    /// <inheritdoc />
    public partial class AddEnrollmentYear : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EnrollmentYear",
                table: "Students",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE Students
                SET EnrollmentYear =
                    2000 + CAST(substr(StudentCode, 1, 2) AS INTEGER)
                WHERE length(StudentCode) = 10
                  AND substr(StudentCode, 1, 2) GLOB '[0-9][0-9]';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EnrollmentYear",
                table: "Students");
        }
    }
}
