using System.Collections.Generic;
using DAL.Common;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class BookRepository : Repository<Book>, IBookRepository
    {
        public BookRepository(BooksDbContext context) : base(context)
        {
        }

        protected override DbSet<Book> DbSet => Context.Books;
        
        public void BulkAdd(IEnumerable<Book> books)
        {
            var autoDetect = Context.ChangeTracker.AutoDetectChangesEnabled;
            try
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = false;
            
                foreach (var book in books)
                {
                    DbSet.Add(book);                
                }
            
                Context.SaveChanges();
            }
            finally
            {
                Context.ChangeTracker.AutoDetectChangesEnabled = autoDetect;                
            }
        }
    }
}