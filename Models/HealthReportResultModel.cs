namespace hyperTROPHYbuddy.Models
{
    public class HealthReportResultModel
    {
        public double BMI { get; set; }
        public string BMICategory { get; set; }
        public double MaintainCalories { get; set; }
        public double MildLossCalories { get; set; }
        public double LossCalories { get; set; }
        public double MildGainCalories { get; set; }
        public double GainCalories { get; set; }
    }
}
