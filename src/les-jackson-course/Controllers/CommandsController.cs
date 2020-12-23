using System.Collections.Generic;
using AutoMapper;
using Cmd.Data;
using Cmd.Dtos;
using Cmd.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cmd.Controllers
{
    [Route("api/commands")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private readonly ICommanderRepo _commandsRepository;
        private readonly IMapper _mapper;

        public CommandsController(ICommanderRepo commandsRepository, IMapper mapper)
        {
            _commandsRepository = commandsRepository;
            _mapper = mapper;
        }

        // GET api/commands
        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDto>> GetAll()
        {
            var commandItems = _commandsRepository.GetAllCommands();

            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));    
        }

        // GET api/commands/{id}
        [HttpGet("{id}")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _commandsRepository.FirstOrDefaultCommandById(id);
            if (commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(commandItem));
            }

            return NotFound();
        }
    }
}