using System;
using System.Collections.Generic;
using System.Linq;
using Cmd.Models;

namespace Cmd.Data
{
    public class MockCommanderRepo : ICommanderRepo
    {
        private readonly List<Command> _commands = new List<Command>
            {
                new Command(1, "Test command 1", "Line", "Platform"),
                new Command(2, "Test command 2", "Line", "Platform"),
                new Command(3, "Test command 3", "Line", "Platform")
            };

        public IEnumerable<Command> GetAllCommands()
        {
            return _commands;
        }

        public Command FirstOrDefaultCommandById(int id)
        {
            return _commands.FirstOrDefault(i => i.Id == id);
        }

        public bool SaveChanges()
        {
            return true;
        }

        public void CreateCommand(Command cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentException("Command is undefined.", nameof(cmd));
            }

            _commands.Add(cmd);
        }

        public void UpdateCommand(Command cmd)
        {
            var r = _commands.FirstOrDefault(i => i.Id == cmd.Id);
            if (r == null)
            {
                return;
            }

            _commands.Remove(r);
            _commands.Add(cmd);
        }

        public void DeleteCommand(Command cmd)
        {
            if (cmd == null)
            {
                return;
            }

             var r = _commands.FirstOrDefault(i => i.Id == cmd.Id);
            if (r == null)
            {
                return;
            }

            _commands.Remove(r);
        }
    }
}