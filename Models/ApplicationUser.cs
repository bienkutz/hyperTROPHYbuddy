using Microsoft.AspNetCore.Identity;

namespace hyperTROPHYbuddy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ClientWorkoutPlan> ClientWorkoutPlans { get; set; }
        public virtual ICollection<Exercise> Exercises { get; set; }
        public virtual ICollection<Workout> Workouts { get; set; }
        public virtual ICollection<WorkoutPlan> WorkoutPlans { get; set; }
        public virtual ICollection<WorkoutLog> WorkoutLogs { get; set; }
        public virtual ICollection<SetLog> SetLogs { get; set; }

    }
}
