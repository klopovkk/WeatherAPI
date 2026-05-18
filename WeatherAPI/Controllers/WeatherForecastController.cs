using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Services;

namespace WeatherAPI.Controllers
{
    [ApiController]
    [Route("weather")]
    [Produces("application/json")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly List<WeatherModel> WeatherRecords = new();

        private readonly WeatherReportService _weatherReportService;

        public WeatherForecastController(
            WeatherReportService weatherReportService)
        {
            _weatherReportService = weatherReportService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                data = WeatherRecords,
                total = WeatherRecords.Count
            });
        }

        [HttpGet("{weatherId:int}")]
        public IActionResult GetById(int weatherId)
        {
            var model = WeatherRecords
                .FirstOrDefault(e => e.Id == weatherId);

            return model == null
                ? NotFound(new
                {
                    success = false,
                    message = "Record not found."
                })
                : Ok(model);
        }

        [HttpPost]
        public IActionResult Create(
            [FromBody] WeatherModel weatherData)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            weatherData.Id = WeatherRecords.Count == 0
                ? 1
                : WeatherRecords.Max(e => e.Id) + 1;

            WeatherRecords.Add(weatherData);

            return StatusCode(201, new
            {
                success = true,
                message = "Record created successfully."
            });
        }

        [HttpPut("{id:int}")]
        public IActionResult Update(
            int id,
            [FromBody] WeatherModel obj)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = WeatherRecords
                .FirstOrDefault(x => x.Id == id);

            if (existing == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Record not found."
                });
            }

            existing.Date = obj.Date;
            existing.TemperatureC = obj.TemperatureC;
            existing.Summary = obj.Summary;

            return Ok(new
            {
                success = true,
                result = existing
            });
        }

        [HttpDelete("{id:int}")]
        public IActionResult Delete(int id)
        {
            var weather = WeatherRecords
                .FirstOrDefault(x => x.Id == id);

            if (weather == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "Record not found."
                });
            }

            WeatherRecords.Remove(weather);

            return Ok(new
            {
                success = true,
                message = "Deleted."
            });
        }

        [HttpGet("summary/report")]
        public IActionResult GetReport()
        {
            var report =
                _weatherReportService
                    .GenerateReport(WeatherRecords);

            return Ok(report);
        }
    }
}