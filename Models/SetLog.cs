namespace hyperTROPHYbuddy.Models
{
    public class SetLog
    {
        public int Id { get; set; }
        public int WorkoutLogId { get; set; }
        public int ExerciseId { get; set; }
        public int SetNumber { get; set; }
        public int Reps { get; set; }
        public decimal Weight { get; set; }
        public string ClientId { get; set; }
        // Navigation properties
        public WorkoutLog WorkoutLog { get; set; }
        public Exercise Exercise { get; set; }
        public ApplicationUser Client { get; set; }
    }
}
