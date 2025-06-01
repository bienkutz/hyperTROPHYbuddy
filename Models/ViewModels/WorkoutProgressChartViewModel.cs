namespace hyperTROPHYbuddy.Models.ViewModels
{
    public class WorkoutProgressChartViewModel
    {
        public List<string> Exercises { get; set; }
        public List<string> Dates { get; set; }
        public List<ChartEntry> ChartData { get; set; }

        public class ChartEntry
        {
            public string Exercise { get; set; }
            public string Date { get; set; }
            public double TotalWeight { get; set; }
        }
    }
}
