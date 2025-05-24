using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class WorkoutPlanWorkout
    {
        public int WorkoutPlanId { get; set; }
        public WorkoutPlan WorkoutPlan { get; set; }

        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }

        [Range(1, 7, ErrorMessage = "Day must be between 1 and 7.")]
        public int Day { get; set; }
    }
}
