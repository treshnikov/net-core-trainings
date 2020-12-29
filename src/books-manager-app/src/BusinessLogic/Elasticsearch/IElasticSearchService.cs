using System.Collections.Generic;
using Domain;

namespace BusinessLogic.Elasticsearch
{
    public interface IElasticSearchService
    {
        void IndexBook(Book book);
        void IndexBooks(IEnumerable<Book> books);
        IReadOnlyCollection<EsBookModel> Search(string query, int skip, int take);
        void CheckIndex();
        void UpdateBook(int bookId, Book book);
    }
}