namespace hyperTROPHYbuddy.Models
{
    public class UserWorkoutPlan
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Client user ID
        public int WorkoutPlanId { get; set; }
        public DateTime AssignedDate { get; set; }
        // Navigation properties
        public WorkoutPlan WorkoutPlan { get; set; }
        //public ApplicationUser User { get; set; }
        public ICollection<WorkoutLog> WorkoutLogs { get; set; }
    }
}
