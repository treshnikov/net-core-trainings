using System;
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
        [HttpGet("{id}", Name="GetCommandById")]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _commandsRepository.FirstOrDefaultCommandById(id);
            if (commandItem != null)
            {
                return Ok(_mapper.Map<CommandReadDto>(commandItem));
            }

            return NotFound();
        }

        //POST api/commands
        [HttpPost]
        public ActionResult<CommandCreateDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var command = _mapper.Map<Command>(commandCreateDto);
            _commandsRepository.CreateCommand(command);
            _commandsRepository.SaveChanges();

            var res = _mapper.Map<CommandReadDto>(command);

            return CreatedAtRoute(nameof(GetCommandById), new {Id = res.Id}, res);
        }

        //PUT api/commands/{id}
        [HttpPut("{id}")]
        public ActionResult UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var command = _commandsRepository.FirstOrDefaultCommandById(id);
            if (command == null)
            {
                return NotFound();
            }

            _mapper.Map(commandUpdateDto, command);
            _commandsRepository.UpdateCommand(command);
            _commandsRepository.SaveChanges();

            return NoContent();
        }
    }
}