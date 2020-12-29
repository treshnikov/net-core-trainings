using System.Collections.Generic;
using System.Linq;
using BusinessLogic;
using BusinessLogic.BookManager;
using BusinessLogic.Elasticsearch;
using DAL.Repositories;
using Domain;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace Tests.UnitTests
{
    [TestFixture]
    public class BookManagerTest
    {
        [Test]
        public void BulkAdd_WithTwoBooks_ShouldAddTwoItems()
        {
            // Arrange
            var repository = new Mock<IBookRepository>();
            var manager = new BooksManager(repository.Object, null, null);

            const string data = @"h0,h1,h2,h3,h4,h5,h6,h7,h8,h9
,,,,,,,Author 1,,Title 1,,
,,,,,,,Author 2,,Title 2,,";
            
            // Act
            var result = manager.BulkAdd(true, data).ToArray();
            
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Length.ShouldBe(2),
                () => result[0].Result.ShouldBe(AddResultValue.Added),
                () => result[0].Book.Title.ShouldBe("Title 1"),
                () => result[0].Book.Author.ShouldBe("Author 1"),
                () => result[1].Result.ShouldBe(AddResultValue.Added),
                () => result[1].Book.Title.ShouldBe("Title 2"),
                () => result[1].Book.Author.ShouldBe("Author 2"));
            
            repository.Verify(m => m.Add(It.IsAny<Book>()), Times.Never);
            repository.Verify(m => m.BulkAdd(It.IsAny<IEnumerable<Book>>()), Times.Once);
        }
        
        [Test]
        public void BulkAdd_WithInvalidCsvFormat_ShouldReturnError()
        {
            // Arrange
            var repository = new Mock<IBookRepository>();
            var manager = new BooksManager(repository.Object, null, null);

            const string data = @"h0;h1;h2;h3;h4;h5;h6;h7;h8;h9
;;;;;;;Author 1;;Title 1;;
;;;;;;;Author 2;;Title 2;;";
            
            // Act
            var result = manager.BulkAdd(true, data).ToArray();
            
            // Assert
            result.ShouldSatisfyAllConditions(
                () => result.Length.ShouldBe(2),
                () => result[0].Result.ShouldBe(AddResultValue.InvalidFormat),
                () => result[1].Result.ShouldBe(AddResultValue.InvalidFormat));
        }

        /// <summary>
        /// Verifies that result of search should be built based on Elasticsearch order
        /// </summary>
        [Test]
        public void Search_ShouldReturnItemsInOrder()
        {
            // Arrange
            var elastic = new Mock<IElasticSearchService>();
            elastic.Setup(m => m.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns<string, int, int>((q, s, t) => new[]
                {
                    new EsBookModel {Id = 2},
                    new EsBookModel {Id = 1}
                });
            
            var repository = new Mock<IBookRepository>();
            repository.Setup(m => m.Query())
                .Returns(new EnumerableQuery<Book>(new []
                {
                    new Book {Id = 1},
                    new Book {Id = 2}
                }));
            
            var manager = new BooksManager(repository.Object, null, elastic.Object);
            
            // Act
            var result = manager.Search("qwe", 0, 10).Select(b => b.Id).ToArray();

            // Assert
            result.ShouldBe(new [] { 2, 1 });
        }
    }
}