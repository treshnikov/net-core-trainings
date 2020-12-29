using System;

namespace JoggingWebApp.Models
{
    public class JoggingDataUpdateModel
    {
        public DateTime? Date { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int? Distance { get; set; }

        public int? Time { get; set; }
    }
}