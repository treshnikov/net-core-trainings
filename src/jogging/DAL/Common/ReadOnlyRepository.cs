using System.Linq;
using BusinessLogic.DAL;
using Microsoft.EntityFrameworkCore;

namespace DAL.Common
{
    public abstract class ReadOnlyRepository<T> : IReadOnlyRepository<T>
        where T: class 
    {
        private readonly JoggingDbContext _context;
        
        protected abstract DbSet<T> DbSet { get; }

        protected ReadOnlyRepository(JoggingDbContext context)
        {
            _context = context;
        }
        
        public IQueryable<T> Query()
        {
            return DbSet;
        }

        public T Find<TInt>(TInt id)
        {
            return DbSet.Find(id);
        }
    }
}