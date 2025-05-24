using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class Workout
    {
        [Key]
        public int WorkoutId { get; set; }
        public string Name { get; set; }

        [ForeignKey("CreatedBy")]
        public string? CreatedByAdminId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        // Navigation properties

        public ICollection<WorkoutPlanWorkout>? WorkoutPlanWorkouts { get; set; } 
        public ICollection<WorkoutExercise>? WorkoutExercises { get; set; }


    }

    


}
