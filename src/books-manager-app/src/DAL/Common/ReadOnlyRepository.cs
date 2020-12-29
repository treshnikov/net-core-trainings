using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DAL.Common
{
    public abstract class ReadOnlyRepository<T> : IReadOnlyRepository<T>
        where T: class
    {
        protected abstract DbSet<T> DbSet { get; }
        
        public IQueryable<T> Query()
        {
            return DbSet;
        }
    }
}