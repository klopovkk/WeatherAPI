using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace WeatherAPI
{
    public class WeatherModel
    {
        public int Id { get; set; }

        [Required]
        public DateOnly Date { get; set; }

        [Range(-50,50)]
        public int TemperatureC { get; set; }

        [StringLength(50)]
        public string? Summary { get; set; } = String.Empty;
    }
}
