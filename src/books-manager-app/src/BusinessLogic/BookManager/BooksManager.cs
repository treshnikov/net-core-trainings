using System.Collections.Generic;
using System.Linq;
using BusinessLogic.Elasticsearch;
using BusinessLogic.Exceptions;
using DAL.Repositories;
using Domain;
using Microsoft.EntityFrameworkCore;
using TinyCsvParser;

namespace BusinessLogic.BookManager
{
    public class BooksManager : IBooksManager
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookReviewRepository _bookReviewRepository;
        private readonly IElasticSearchService _elasticSearchService;

        public BooksManager(
            IBookRepository bookRepository, 
            IBookReviewRepository bookReviewRepository, 
            IElasticSearchService elasticSearchService)
        {
            _bookRepository = bookRepository;
            _bookReviewRepository = bookReviewRepository;
            _elasticSearchService = elasticSearchService;
        }
        
        public IReadOnlyCollection<Book> Get(int? skip, int? take)
        {
            IQueryable<Book> query = _bookRepository.Query()
                .Include(b => b.Reviews);

            if (skip.HasValue)
            {
                query = query.Skip(skip.Value);
            }

            if (take.HasValue)
            {
                query = query.Take(take.Value);
            }

            return query.ToArray();
        }

        public Book Get(int id)
        {
            return _bookRepository.Query()
                       .Include(b => b.Reviews)
                       .FirstOrDefault(b => b.Id == id) ??
                   throw new NotFoundException("Book not found");
        }

        public void Add(Book book)
        {
            book.Reviews ??= new BookReview[0];
            _bookRepository.Add(book);
            _elasticSearchService.IndexBook(book);
        }

        public void Update(Book book)
        {
            book.Reviews ??= new BookReview[0];
            _bookRepository.Update(book.Id, book);
            _elasticSearchService.UpdateBook(book.Id, book);
        }

        public void Delete(int bookId)
        {
            _bookRepository.Delete(bookId);
        }

        public IReadOnlyCollection<AddResult> BulkAdd(bool skipHeader, string data)
        {
            var csvReadOptions = new CsvReaderOptions(new[] {"\r", "\n", "\r\n"});
            var csvParserOptions = new CsvParserOptions(skipHeader, ',');
            var csvMapper = new CsvBookMapping();
            var csvParser = new CsvParser<Book>(csvParserOptions, csvMapper);

            var parseResults = csvParser
                .ReadFromString(csvReadOptions, data)
                .ToArray();

            var result = new List<AddResult>();
            foreach (var parseResult in parseResults)
            {
                if (!parseResult.IsValid)
                {
                    result.Add(new AddResult
                    {
                        Result = AddResultValue.InvalidFormat
                    });
                    
                    continue;
                }
                
                result.Add(new AddResult
                {
                    Book = parseResult.Result,
                    Result = AddResultValue.Added
                });
            }

            var booksToAdd = result.Where(r => r.Result == AddResultValue.Added).Select(r => r.Book);
            _bookRepository.BulkAdd(booksToAdd);
            
            _elasticSearchService.IndexBooks(booksToAdd);

            return result;
        }

        public IReadOnlyCollection<Book> Search(string query, int skip, int take)
        {
            var elasticResult = _elasticSearchService.Search(query, skip, take);
            var ids = elasticResult.Select(b => b.Id).ToArray();
            var map = ids
                .Select((id, index) => new { id, index})
                .ToDictionary(p => p.id, p => p.index);
            
            return _bookRepository.Query()
                .Where(b => ids.Contains(b.Id))
                .Include(b => b.Reviews)
                .AsEnumerable()
                .OrderBy(b => map[b.Id])
                .ToArray();
        }

        public void AddReview(int bookId, BookReview review)
        {
            var book = _bookRepository.Query().FirstOrDefault(b => b.Id == bookId) ??
                       throw new NotFoundException("Book not found");

            review.Book = book;
            _bookReviewRepository.Add(review);
        }
    }
}