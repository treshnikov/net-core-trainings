using System.Collections.Generic;
using System.Linq;
using Cmd.Models;

namespace Cmd.Data
{
    public class SqlCommanderRepository : ICommanderRepo
    {
        private readonly CommanderContext _context;

        public SqlCommanderRepository(CommanderContext context)
        {
            _context = context;
        }

        public IEnumerable<Command> GetAllCommands()
        {
            return _context.Commands.ToList();
        }

        public Command FirstOrDefaultCommandById(int id)
        {
            return _context.Commands.FirstOrDefault(i => i.Id == id);
        }
    }
}