using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanType> WorkoutPlanTypes { get; set; }
        public DbSet<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
        public DbSet<UserWorkoutPlan> UserWorkoutPlans { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }
        public DbSet<SetLog> SetLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure many-to-many relationships
            modelBuilder.Entity<WorkoutExercise>()
                .HasKey(we => new { we.WorkoutId, we.ExerciseId });

            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasKey(wpw => new { wpw.WorkoutPlanId, wpw.WorkoutId });

            modelBuilder.Entity<SetLog>()
                .Property(s => s.Weight)
                .HasColumnType("decimal(5, 2)"); 

            // Seed initial workout plan types
            modelBuilder.Entity<WorkoutPlanType>().HasData(
                new WorkoutPlanType { Id = 1, Name = "Weight Loss" },
                new WorkoutPlanType { Id = 2, Name = "Hypertrophy" },
                new WorkoutPlanType { Id = 3, Name = "HIIT" },
                new WorkoutPlanType { Id = 4, Name = "Powerlifting" }
            );
        }
    }
}
