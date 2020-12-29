using Microsoft.EntityFrameworkCore;

namespace DAL.Common
{
    public abstract class Repository<T> : AddableRepository<T>, IRepository<T> 
        where T: class
    {
        protected Repository(BooksDbContext context) : base(context)
        {
            
        }
        
        public void Update(int id, T entity)
        {
            var old = DbSet.Find(id);
            Context.Entry(old).State = EntityState.Detached;
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = DbSet.Find(id);
            Context.Remove(entity);
            Context.SaveChanges();
        }
    }
}