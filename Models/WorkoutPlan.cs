using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class WorkoutPlan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int WorkoutPlanTypeId { get; set; }
        public string AdminId { get; set; }
        // Navigation properties
        public WorkoutPlanType WorkoutPlanType { get; set; }
        public ICollection<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
        public ICollection<ClientWorkoutPlan> ClientWorkoutPlans { get; set; }
        public ApplicationUser Admin { get; set; }

        // For the binding of the selected workouts in the view

        [NotMapped]
        public List<int> SelectedWorkoutIds { get; set; } = new List<int>();
    }
}
