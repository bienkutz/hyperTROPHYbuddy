using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace hyperTROPHYbuddy.Models
{
    public class ExerciseLog
    {
        public int ExerciseLogId { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public int WorkoutId { get; set; }
        public Workout Workout { get; set; }

        [Required]
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }

        [Required]
        public int SetNumber { get; set; }

        [Required]
        public int Reps { get; set; }

        [Required]
        public double Weight { get; set; }

        public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
    }
}