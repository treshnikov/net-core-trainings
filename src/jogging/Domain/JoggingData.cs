using System;
using System.ComponentModel.DataAnnotations;
using Domain.Attributes;

namespace Domain
{
    public class JoggingData
    {
        public long Id { get; set; }
        
        public DateTime Date { get; set; }
        
        public double Latitude { get; set; }
        
        public double Longitude { get; set; }
        
        public int Distance { get; set; }
        
        public int Time { get; set; }

        [Required]
        [NonFilterable]
        public User User { get; set; }

        [NonFilterable]
        public string WeatherInfo { get; set; }
    }
}