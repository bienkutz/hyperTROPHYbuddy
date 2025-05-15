namespace hyperTROPHYbuddy.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string VideoLink { get; set; }
        public string AdminId { get; set; } // Assuming you'll use ASP.NET Identity later
                                            
        // Navigation property
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
    }
}
