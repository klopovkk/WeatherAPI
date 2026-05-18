using WeatherAPI.DTOs;

namespace WeatherAPI.Services
{
    public class WeatherReportService
    {
        public WeatherSummaryReportDto GenerateReport(List<WeatherModel> records)
        {
            return new WeatherSummaryReportDto
            {
                TotalRecords = records.Count,
                AverageTemperature = records.Any()
                    ? records.Average(x => x.TemperatureC)
                    : 0,

                Hottest = records
                    .OrderByDescending(x => x.TemperatureC)
                    .FirstOrDefault(),

                Coldest = records
                    .OrderBy(x => x.TemperatureC)
                    .FirstOrDefault(),

                MostFrequentCondition = records
                    .GroupBy(x => x.Summary)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .FirstOrDefault()
            };
        }
    }
}