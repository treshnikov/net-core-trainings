using DAL.Common;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ServerSettingsRepository : ReadOnlyRepository<ServerSettings>
    {
        private readonly JoggingDbContext _context;

        public ServerSettingsRepository(JoggingDbContext context) : base(context)
        {
            _context = context;
        }

        protected override DbSet<ServerSettings> DbSet => _context.ServerSettings;
    }
}