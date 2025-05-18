namespace hyperTROPHYbuddy.Models
{
    public class WorkoutLog
    {
        public int Id { get; set; }
        public int ClientWorkoutPlanId { get; set; }
        public int WorkoutId { get; set; }
        public DateTime Date { get; set; }
        public string ClientId { get; set; } // Client user ID
        // Navigation properties
        public ClientWorkoutPlan ClientWorkoutPlan { get; set; }
        public Workout Workout { get; set; }
        public ICollection<SetLog> SetLogs { get; set; }
        public ApplicationUser Client { get; set; }
    }
}
