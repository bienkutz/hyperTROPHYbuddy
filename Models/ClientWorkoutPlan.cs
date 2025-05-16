namespace hyperTROPHYbuddy.Models
{
    public class ClientWorkoutPlan
    {
        public int Id { get; set; }
        public string ClientId { get; set; } // Client user ID
        public int WorkoutPlanId { get; set; }
        public DateTime AssignedDate { get; set; }

        // Navigation properties
        public WorkoutPlan WorkoutPlan { get; set; }
        public ApplicationUser Client { get; set; }
        public ICollection<WorkoutLog> WorkoutLogs { get; set; }
    }
}
