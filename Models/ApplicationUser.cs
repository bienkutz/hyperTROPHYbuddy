using Microsoft.AspNetCore.Identity;

namespace hyperTROPHYbuddy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;
        public int? LastNotifiedWorkoutPlanAssignmentId { get; set; }

        // Navigation properties

        public ICollection<WorkoutPlanAssignment>? AssignedToMe { get; set; } //for clients

        public ICollection<WorkoutPlanAssignment>? AssignedByMe { get; set; } //for admins

        public ICollection<Workout>? CreatedWorkouts { get; set; } 
        public ICollection<Exercise>? CreatedExercises { get; set; }
        public ICollection<WorkoutPlan>? CreatedWorkoutPlans { get; set; }

    }
}
