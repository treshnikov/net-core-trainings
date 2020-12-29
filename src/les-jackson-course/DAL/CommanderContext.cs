using Cmd.Models;
using Microsoft.EntityFrameworkCore;

namespace Cmd.Data
{
    public class CommanderContext : DbContext
    {
        public CommanderContext(DbContextOptions<CommanderContext> options) : base(options)
        {
        }

        public DbSet<Command> Commands { get; set; }
    }
}