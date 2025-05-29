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

        [Range(1, 10, ErrorMessage = "Target sets must be between 1 and 10.")]
        public int TargetSets { get; set; }

        [ForeignKey("CreatedBy")]
        public string? CreatedByAdminId { get; set; }
        public ApplicationUser? CreatedBy { get; set; }

        // Navigation property
        public ICollection<WorkoutExercise>? WorkoutExercises { get; set; } 

    }
}
