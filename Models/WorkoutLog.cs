namespace hyperTROPHYbuddy.Models
{
    public class WorkoutLog
    {
        public int Id { get; set; }
        public int UserWorkoutPlanId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime Date { get; set; }
        // Navigation properties
        public UserWorkoutPlan UserWorkoutPlan { get; set; }
        public Workout Workout { get; set; }
        public ICollection<SetLog> SetLogs { get; set; }
    }
}
