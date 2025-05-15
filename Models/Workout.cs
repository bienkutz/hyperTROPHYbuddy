using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string AdminId { get; set; }
        // Navigation properties
        public ICollection<WorkoutExercise> WorkoutExercises { get; set; }
        public ICollection<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }

        //For the binding of the selected exercises in the view
        [NotMapped]
        public List<int> SelectedExerciseIds { get; set; } = new List<int>();
    }

    // Junction table for Workout-Exercise many-to-many
    public class WorkoutExercise
    {
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
    }


}
