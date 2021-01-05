using System.ComponentModel.DataAnnotations;

namespace Cmd.Dtos
{
    public class CommandReadDto{
        public CommandReadDto(int id, string howTo, string line)
        {
            Id = id;
            HowTo = howTo;
            Line = line;
        }

        public int Id {get; init;} 
        public string HowTo {get; init;}
        public string Line {get; init;}
    }
    
}