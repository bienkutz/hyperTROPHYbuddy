using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class Exercise
    {
        [Key]
        public int ExerciseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string? VideoLink { get; set; }

        [ForeignKey("CreatedBy")]
        public string? CreatedByAdminId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        // Navigation property
        public ICollection<WorkoutExercise>? WorkoutExercises { get; set; } 

    }
}
