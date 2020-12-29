using System.Collections.Generic;
using Domain;

namespace BusinessLogic.BookManager
{
    public interface IBooksManager
    {
        IReadOnlyCollection<Book> Get(int? skip, int? take);
        Book Get(int id);
        void Add(Book book);
        void Update(Book book);
        void Delete(int bookId);
        IReadOnlyCollection<AddResult> BulkAdd(bool skipHeader, string data);
        IReadOnlyCollection<Book> Search(string query, int skip, int take);
        void AddReview(int bookId, BookReview review);
    }
}