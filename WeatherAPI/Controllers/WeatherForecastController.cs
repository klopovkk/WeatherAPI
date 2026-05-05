using Microsoft.AspNetCore.Mvc;

namespace WeatherAPI.Controllers
{
    [ApiController]
    [Route("weather")]
    [Produces("application/json")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly List<WeatherModel> WeatherRecords = new();

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<WeatherModel>), StatusCodes.Status200OK)]
        public IActionResult Get()
        {
            return Ok(WeatherRecords);
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(WeatherModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult GetById(int id)
        {
            WeatherModel? model = WeatherRecords.FirstOrDefault(e => e.Id == id);

            return model == null ? NotFound() : Ok(model);
        }

        [HttpPost]
        [ProducesResponseType(typeof(WeatherModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult Create([FromBody] WeatherModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            model.Id = WeatherRecords.Count == 0
                ? 1
                : WeatherRecords.Max(e => e.Id) + 1;

            WeatherRecords.Add(model);

            return CreatedAtAction(nameof(GetById), new { id = model.Id }, model);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(WeatherModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Update(int id, [FromBody] WeatherModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existing = WeatherRecords.FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return NotFound();

            existing.Date = model.Date;
            existing.TemperatureC = model.TemperatureC;
            existing.Summary = model.Summary;

            return Ok(existing);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult Delete(int id)
        {
            var existing = WeatherRecords.FirstOrDefault(x => x.Id == id);

            if (existing == null)
                return NotFound();

            WeatherRecords.Remove(existing);

            return NoContent();
        }
    }
}
