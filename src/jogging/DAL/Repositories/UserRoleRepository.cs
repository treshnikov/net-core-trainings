using DAL.Common;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class UserRoleRepository : ReadOnlyRepository<UserRole>
    {
        private readonly JoggingDbContext _context;

        public UserRoleRepository(JoggingDbContext context) : base(context)
        {
            _context = context;
        }

        protected override DbSet<UserRole> DbSet => _context.UserRoles;
    }
}