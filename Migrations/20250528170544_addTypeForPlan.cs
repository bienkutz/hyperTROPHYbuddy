using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hyperTROPHYbuddy.Migrations
{
    /// <inheritdoc />
    public partial class addTypeForPlan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "WorkoutPlans",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "WorkoutPlans");
        }
    }
}
