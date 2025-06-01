using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hyperTROPHYbuddy.Migrations
{
    /// <inheritdoc />
    public partial class testCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByAdminId",
                table: "WorkoutPlans",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkoutPlans_CreatedByAdminId",
                table: "WorkoutPlans",
                column: "CreatedByAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkoutPlans_AspNetUsers_CreatedByAdminId",
                table: "WorkoutPlans",
                column: "CreatedByAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkoutPlans_AspNetUsers_CreatedByAdminId",
                table: "WorkoutPlans");

            migrationBuilder.DropIndex(
                name: "IX_WorkoutPlans_CreatedByAdminId",
                table: "WorkoutPlans");

            migrationBuilder.DropColumn(
                name: "CreatedByAdminId",
                table: "WorkoutPlans");
        }
    }
}
