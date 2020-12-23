using System.Collections.Generic;
using Cmd.Models;

namespace Cmd.Data
{
    public class MockCommanderRepo : ICommanderRepo
    {
        public IEnumerable<Command> GetAllCommands()
        {
            var commands = new List<Command>()
            {
                new Command(1, "Test command 1", "Line", "Platform"),
                new Command(2, "Test command 2", "Line", "Platform"),
                new Command(3, "Test command 3", "Line", "Platform")
            };

            return commands;
        }

        public Command FirstOrDefaultCommandById(int id)
        {
            return new Command(1, "Test command 1", "Line", "Platform");
        }
    }
}