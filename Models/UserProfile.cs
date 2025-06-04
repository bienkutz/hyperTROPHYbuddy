
using System.ComponentModel.DataAnnotations;

namespace hyperTROPHYbuddy.Models
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }
        public string? DietPreference { get; set; }
        public ICollection<UserAllergy> Allergies { get; set; } = new List<UserAllergy>();
        public ICollection<MealFeedback> MealFeedbacks { get; set; } = new List<MealFeedback>();
    }

    public class UserAllergy
    {
        [Key]
        public int Id { get; set; }
        public string Allergy { get; set; }
        public string UserId { get; set; }
    }
    public class MealFeedback
    {
        [Key]
        public int Id { get; set; }
        public string MealId { get; set; }
        public string UserId { get; set; }
        public bool Liked { get; set; }
    }
}
