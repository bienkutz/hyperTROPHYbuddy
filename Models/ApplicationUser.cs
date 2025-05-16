using Microsoft.AspNetCore.Identity;

namespace hyperTROPHYbuddy.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime RegisteredAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ClientWorkoutPlan> ClientWorkoutPlans { get; set; }

    }
}
