namespace WeatherAPI.DTOs
{
    public class WeatherSummaryReportDto
    {
        public int TotalRecords { get; set; }

        public double AverageTemperature { get; set; }

        public WeatherModel? Hottest { get; set; }

        public WeatherModel? Coldest { get; set; }

        public string? MostFrequentCondition { get; set; }
    }
}