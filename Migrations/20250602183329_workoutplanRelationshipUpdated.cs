using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hyperTROPHYbuddy.Migrations
{
    /// <inheritdoc />
    public partial class workoutplanRelationshipUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutPlanAssignments_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutPlanAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutPlanAssignments_WorkoutPlans_WorkoutPlanId1",
                table: "WorkoutPlanAssignments");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutPlanAssignments_WorkoutPlanId1",
                table: "WorkoutPlanAssignments");

            migrationBuilder.DropColumn(
                name: "WorkoutPlanId1",
                table: "WorkoutPlanAssignments");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutPlanAssignments_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutPlanAssignments",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "WorkoutPlanId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutPlanAssignments_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutPlanAssignments");

            migrationBuilder.AddColumn<int>(
                name: "WorkoutPlanId1",
                table: "WorkoutPlanAssignments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlanAssignments_WorkoutPlanId1",
                table: "WorkoutPlanAssignments",
                column: "WorkoutPlanId1");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutPlanAssignments_WorkoutPlans_WorkoutPlanId",
                table: "WorkoutPlanAssignments",
                column: "WorkoutPlanId",
                principalTable: "WorkoutPlans",
                principalColumn: "WorkoutPlanId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutPlanAssignments_WorkoutPlans_WorkoutPlanId1",
                table: "WorkoutPlanAssignments",
                column: "WorkoutPlanId1",
                principalTable: "WorkoutPlans",
                principalColumn: "WorkoutPlanId");
        }
    }
}
