using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusinessLogic.Common;
using Domain;
using Microsoft.Extensions.Options;
using Nest;

namespace BusinessLogic.Elasticsearch
{
    public class ElasticSearchService : IElasticSearchService
    {
        private const string IndexName = "books";
        private readonly ElasticClient _client;

        public ElasticSearchService(IOptions<AppSettings> appSettings)
        {
            var settings = new ConnectionSettings(new Uri(appSettings.Value.ElasticsearchUrl))
                .DefaultIndex(IndexName)
                .ThrowExceptions();

            _client = new ElasticClient(settings);
        }

        public void IndexBook(Book book)
        {
            var model = new EsBookModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author
            };
            
            _client.IndexDocument(model);
        }

        public void IndexBooks(IEnumerable<Book> books)
        {
            _client.IndexMany(books.Select(b => new EsBookModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author
            }));
        }

        public IReadOnlyCollection<EsBookModel> Search(string query, int skip, int take)
        {
            var response = _client.Search<EsBookModel>(s => s
                .Index(IndexName)
                .From(skip)
                .Size(take)
                .Query(q => q
                    .MultiMatch(m => m
                        .Operator(Operator.And)
                        .Fields(fs => fs.Field(f => f.Title, 6).Field(f => f.Author, 1))
                        .Query(query))));

            return response.Documents;
        }

        /// <summary>
        /// Checks index exists and creates new index with settings if needed
        /// </summary>
        public void CheckIndex()
        {
            Task.Run(() =>
            {
                var indexResult = _client.Indices.Get(Indices.AllIndices, i => i.Index(IndexName));
                if (!indexResult.Indices.ContainsKey(IndexName))
                {
                    const string analyzerName = "myanalyzer";
                    const string tokenizerName = "myngram";
                    
                    _client.Indices.Create(IndexName, c => c
                        .Settings(s => s
                            .Analysis(a => a
                                .Tokenizers(t =>
                                    t.NGram(tokenizerName,
                                        ng => ng.MinGram(3).MaxGram(4).TokenChars(TokenChar.Letter, TokenChar.Digit)))
                                .Analyzers(aa => aa
                                    .Custom(analyzerName, c =>
                                        c.Tokenizer(tokenizerName).Filters("lowercase")))
                            )
                        )
                        .Map<Book>(mm => mm
                            .Properties(p => p
                                .Text(t => t
                                    .Name(n => n.Title)
                                    .Analyzer(analyzerName)
                                )
                            )
                            .Properties(p => p
                                .Text(t => t
                                    .Name(n => n.Author)
                                    .Analyzer(analyzerName)))
                        )
                    );
                }
            });
        }

        public void UpdateBook(int bookId, Book book)
        {
            _client.Update<EsBookModel, object>(bookId, u => u
                .Index(IndexName)
                .Doc(new
                {
                    book.Title,
                    book.Author
                }));
        }
    }
}