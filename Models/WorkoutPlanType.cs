namespace hyperTROPHYbuddy.Models
{
    public class WorkoutPlanType
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Weight Loss", "Hypertrophy", etc.
                                         
        // Navigation property
        public ICollection<WorkoutPlan> WorkoutPlans { get; set; }
    }

    // Junction table for WorkoutPlan-Workout many-to-many
    public class WorkoutPlanWorkout
    {
        public int WorkoutPlanId { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }
    }
}
