using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hyperTROPHYbuddy.Migrations
{
    /// <inheritdoc />
    public partial class AddLastNotifiedWorkoutPlanAssignmentIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastNotifiedWorkoutPlanAssignmentId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastNotifiedWorkoutPlanAssignmentId",
                table: "AspNetUsers");
        }
    }
}
