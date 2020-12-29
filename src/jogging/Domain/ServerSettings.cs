using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class ServerSettings
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        public string Value { get; set; }
    }
}