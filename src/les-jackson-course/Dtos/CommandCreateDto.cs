using System.ComponentModel.DataAnnotations;

namespace Cmd.Dtos
{
    public class CommandCreateDto{
        public CommandCreateDto(string howTo, string line, string platform)
        {
            HowTo = howTo;
            Line = line;
            Platform = platform;
        }

        public string HowTo {get; init;}
        public string Line {get; init;}
        public string Platform {get; init;}
    }
    
}