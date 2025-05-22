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

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Workout> Workouts { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutPlanType> WorkoutPlanTypes { get; set; }
        public DbSet<WorkoutPlanWorkout> WorkoutPlanWorkouts { get; set; }
        public DbSet<ClientWorkoutPlan> ClientWorkoutPlans { get; set; }
        public DbSet<WorkoutLog> WorkoutLogs { get; set; }
        public DbSet<SetLog> SetLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // This is crucial for Identity tables


            // Exercise relationships
            modelBuilder.Entity<Exercise>()
                .HasOne(e => e.Admin)
                .WithMany(a => a.Exercises)
                .HasForeignKey(e => e.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // Workout relationships
            modelBuilder.Entity<Workout>()
                .HasOne(w => w.Admin)
                .WithMany(a => a.Workouts)
                .HasForeignKey(w => w.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkoutPlan relationships
            modelBuilder.Entity<WorkoutPlan>()
                .HasOne(wp => wp.Admin)
                .WithMany(a => a.WorkoutPlans)
                .HasForeignKey(wp => wp.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkoutExercise (Junction table configuration)
            modelBuilder.Entity<WorkoutExercise>()
                .HasKey(we => new { we.WorkoutId, we.ExerciseId });

            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Workout)
                .WithMany(w => w.WorkoutExercises)
                .HasForeignKey(we => we.WorkoutId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.Exercise)
                .WithMany(e => e.WorkoutExercises)
                .HasForeignKey(we => we.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkoutPlanWorkout (Junction table configuration)
            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasKey(wpw => new { wpw.WorkoutPlanId, wpw.WorkoutId });

            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasOne(wpw => wpw.WorkoutPlan)
                .WithMany(wp => wp.WorkoutPlanWorkouts)
                .HasForeignKey(wpw => wpw.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkoutPlanWorkout>()
                .HasOne(wpw => wpw.Workout)
                .WithMany(w => w.WorkoutPlanWorkouts)
                .HasForeignKey(wpw => wpw.WorkoutId)
                .OnDelete(DeleteBehavior.Restrict);

            // ClientWorkoutPlan relationships
            modelBuilder.Entity<ClientWorkoutPlan>()
                .HasOne(cwp => cwp.Client)
                .WithMany(c => c.ClientWorkoutPlans)
                .HasForeignKey(cwp => cwp.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ClientWorkoutPlan>()
                .HasOne(cwp => cwp.WorkoutPlan)
                .WithMany(wp => wp.ClientWorkoutPlans)
                .HasForeignKey(cwp => cwp.WorkoutPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // WorkoutLog relationships
            modelBuilder.Entity<WorkoutLog>()
                .HasOne(wl => wl.Client)
                .WithMany(c => c.WorkoutLogs)
                .HasForeignKey(wl => wl.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<WorkoutLog>()
                .HasOne(wl => wl.ClientWorkoutPlan)
                .WithMany(cwp => cwp.WorkoutLogs)
                .HasForeignKey(wl => wl.ClientWorkoutPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<WorkoutLog>()
                .HasOne(wl => wl.Workout)
                .WithMany()
                .HasForeignKey(wl => wl.WorkoutId)
                .OnDelete(DeleteBehavior.Restrict);

            // SetLog relationships
            modelBuilder.Entity<SetLog>()
                .Property(s => s.Weight)
                .HasColumnType("decimal(5, 2)");

            modelBuilder.Entity<SetLog>()
                .HasOne(sl => sl.WorkoutLog)
                .WithMany(wl => wl.SetLogs)
                .HasForeignKey(sl => sl.WorkoutLogId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SetLog>()
                .HasOne(sl => sl.Exercise)
                .WithMany(e => e.SetLogs)
                .HasForeignKey(sl => sl.ExerciseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Seed data
            modelBuilder.Entity<WorkoutPlanType>().HasData(
                new WorkoutPlanType { Id = 1, Name = "Weight Loss" },
                new WorkoutPlanType { Id = 2, Name = "Hypertrophy" },
                new WorkoutPlanType { Id = 3, Name = "HIIT" },
                new WorkoutPlanType { Id = 4, Name = "Powerlifting" }
            );
        }
    }
}
