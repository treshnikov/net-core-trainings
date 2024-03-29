using System;
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

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void CreateCommand(Command cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentException("Command is undefined.", nameof(cmd));
            }

            _context.Commands.Add(cmd);
        }

        public void UpdateCommand(Command cmd)
        {
            _context.Commands.Update(cmd);
        }

        public void DeleteCommand(Command cmd)
        {
             if (cmd == null)
            {
                throw new ArgumentException("Command is undefined.", nameof(cmd));
            }

            _context.Commands.Remove(cmd);    
        }
    }
}