using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class WorkoutExercise
    {
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }

        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
    }
}
