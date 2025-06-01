using hyperTROPHYbuddy.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace hyperTROPHYbuddy.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkoutPlanAssignment> WorkoutPlanAssignments { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<ExerciseLog> ExerciseLogs { get; set; }        

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<WorkoutExercise>()
                .HasKey(we => new { we.WorkoutId, we.ExerciseId });

            builder.Entity<WorkoutPlanWorkout>()
                .HasKey(wpw => new { wpw.WorkoutPlanId, wpw.WorkoutId });


            // WorkoutPlanAssignment relationships
            builder.Entity<WorkoutPlanAssignment>()
                .HasOne(wpa => wpa.Client)
                .WithMany(u => u.AssignedToMe)
                .HasForeignKey(wpa => wpa.AssignedToClientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkoutPlanAssignment>()
                .HasOne(wpa => wpa.Admin)
                .WithMany(u => u.AssignedByMe)
                .HasForeignKey(wpa => wpa.AssignedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<WorkoutPlanAssignment>()
                .HasOne(wpa => wpa.WorkoutPlan)
                .WithMany()
                .HasForeignKey(wpa => wpa.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Exercise relationships
            builder.Entity<Exercise>()
                .HasOne(e => e.CreatedBy)
                .WithMany(u => u.CreatedExercises)
                .HasForeignKey(e => e.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Workout relationships
            builder.Entity<Workout>()
                .HasOne(w => w.CreatedBy)
                .WithMany(u => u.CreatedWorkouts)
                .HasForeignKey(w => w.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkoutPlan relationships
            builder.Entity<WorkoutPlan>()
                .HasOne(wp => wp.CreatedBy)
                .WithMany(u => u.CreatedWorkoutPlans)
                .HasForeignKey(wp => wp.CreatedByAdminId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
