using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hyperTROPHYbuddy.Migrations
{
    /// <inheritdoc />
    public partial class addingTargets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TargetSets",
                table: "WorkoutExercise",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetSets",
                table: "WorkoutExercise");
        }
    }
}
