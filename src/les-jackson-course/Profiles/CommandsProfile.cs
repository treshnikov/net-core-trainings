using AutoMapper;
using Cmd.Dtos;
using Cmd.Models;

namespace Cmd.Profiles
{
    public class CommandsProfile : Profile
    {
        public CommandsProfile()
        {
            CreateMap<Command, CommandReadDto>();
        }
    }    
}