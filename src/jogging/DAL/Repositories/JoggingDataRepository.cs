using DAL.Common;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class JoggingDataRepository : Repository<JoggingData>
    {
        private readonly JoggingDbContext _context;

        public JoggingDataRepository(JoggingDbContext context) : base(context)
        {
            _context = context;
        }

        protected override DbSet<JoggingData> DbSet => _context.JoggingData;
    }
}