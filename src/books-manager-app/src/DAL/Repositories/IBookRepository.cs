using System.Collections.Generic;
using DAL.Common;
using Domain;

namespace DAL.Repositories
{
    public interface IBookRepository : IRepository<Book>
    {
        void BulkAdd(IEnumerable<Book> books);
    }
}