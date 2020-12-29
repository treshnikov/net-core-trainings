using System.ComponentModel.DataAnnotations;

namespace Cmd.Dtos
{
    public class CommandUpdateDto{
        public CommandUpdateDto(string howTo, string line, string platform)
        {
            HowTo = howTo;
            Line = line;
            Platform = platform;
        }

        [Required]
        [MaxLength(250)]
        public string HowTo {get; init;}

        [Required]
        public string Line {get; init;}

        [Required]
        public string Platform {get; init;}
    }
    
}