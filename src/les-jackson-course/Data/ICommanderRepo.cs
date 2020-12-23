using System.Collections.Generic;
using Cmd.Models;

namespace Cmd.Data
{
    public interface ICommanderRepo
    {
        IEnumerable<Command> GetAllCommands(); 

        Command FirstOrDefaultCommandById(int id);
    }
}