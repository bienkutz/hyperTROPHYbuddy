using System.ComponentModel.DataAnnotations;

namespace hyperTROPHYbuddy.Models
{
    public class HealthReportInputModel
    {
        [Required]
        public string Weight { get; set; } // in kg

        [Required]
        [Range(100, 250)]
        public double Height { get; set; } // in cm

        [Required]
        [Range(5, 120)]
        public int Age { get; set; }

        [Required]
        public string Gender { get; set; }

        [Required]
        public string ActivityLevel { get; set; }
    }
}
