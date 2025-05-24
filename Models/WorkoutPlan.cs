using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class WorkoutPlan
    {
        [Key]
        public int WorkoutPlanId { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<WorkoutPlanAssignment>? Assignments { get; set; } 
        public ICollection<WorkoutPlanWorkout>? WorkoutPlanWorkouts { get; set; }
    }
}
