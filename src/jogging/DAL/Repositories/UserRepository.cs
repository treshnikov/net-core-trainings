using DAL.Common;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRepository : Repository<User>
    {
        private readonly JoggingDbContext _context;

        public UserRepository(JoggingDbContext context) : base(context)
        {
            _context = context;
        }

        protected override DbSet<User> DbSet => _context.Users;
    }
}