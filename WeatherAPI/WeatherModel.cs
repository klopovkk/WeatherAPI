using System.ComponentModel.DataAnnotations;

namespace WeatherAPI
{
    public class WeatherModel
    {
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Range(-100, 100)]
        public int TemperatureC { get; set; }

        [Required]
        [StringLength(50)]
        public string Summary { get; set; } = string.Empty;
    }
}