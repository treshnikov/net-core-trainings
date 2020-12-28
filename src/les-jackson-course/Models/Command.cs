using System.ComponentModel.DataAnnotations;

namespace Cmd.Models
{
    public class Command{
        public Command()
        {
            
        }

        public Command(int id, string howTo, string line, string platform)
        {
            Id = id;
            HowTo = howTo;
            Line = line;
            Platform = platform;
        }

        [Key]
        public int Id {get; init;} 
        [Required]
        [MaxLength(250)]
        public string HowTo {get; init;}
        public string Line {get; init;}
        public string Platform {get; init;}
    }
    
}