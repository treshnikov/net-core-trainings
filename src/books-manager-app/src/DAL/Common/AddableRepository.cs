using Domain;

namespace DAL.Common
{
    public abstract class AddableRepository<T> : ReadOnlyRepository<T>, IAddableRepository<T>
        where T: class
    {
        protected readonly BooksDbContext Context;
        
        protected AddableRepository(BooksDbContext context)
        {
            Context = context;
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges();
        }
    }
}