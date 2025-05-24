using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class WorkoutPlanAssignment
    {
        [Key]
        public int WorkoutPlanAssignmentId { get; set; }

        [ForeignKey("WorkoutPlan")]
        public int? WorkoutPlanId { get; set; }
        public WorkoutPlan? WorkoutPlan { get; set; }

        [ForeignKey("Client")]
        public string? AssignedToClientId { get; set; } 
        public ApplicationUser? Client { get; set; }

        [ForeignKey("Admin")]
        public string? AssignedByAdminId { get; set; }
        public ApplicationUser? Admin { get; set; }

    }
}
