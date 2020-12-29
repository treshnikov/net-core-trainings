using System.Linq;
using BusinessLogic.DAL;
using Microsoft.EntityFrameworkCore;

namespace DAL.Common
{
    public abstract class Repository<T> : ReadOnlyRepository<T>, IRepository<T>
        where T: class 
    {
        private readonly JoggingDbContext _context;
        
        protected Repository(JoggingDbContext context) : base(context)
        {
            _context = context;
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update<TInt>(TInt id, T entity)
        {
            var old = DbSet.Find(id);
            _context.Entry(old).State = EntityState.Detached;
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void Delete(long id)
        {
            var entity = DbSet.Find(id);
            _context.Remove(entity);
            _context.SaveChanges();
        }
    }
}