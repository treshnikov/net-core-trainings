using System.Collections.Generic;
using Cmd.Models;

namespace Cmd.Data
{
    public interface ICommanderRepo
    {
        bool SaveChanges();

        IEnumerable<Command> GetAllCommands(); 
        Command FirstOrDefaultCommandById(int id);
        void CreateCommand(Command cmd);
    }
}