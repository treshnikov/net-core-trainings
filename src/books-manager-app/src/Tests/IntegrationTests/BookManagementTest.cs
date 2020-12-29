using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Elasticsearch;
using Domain;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using WebServer;
using WebServer.Models;

namespace Tests.IntegrationTests
{
    [TestFixture]
    public class BookManagementTest
    {
        private CustomWebApplicationFactory<Startup> _factory;
        private Mock<IElasticSearchService> _elasticMock;

        [SetUp]
        public void Setup()
        {
            _elasticMock = new Mock<IElasticSearchService>();
            _factory = new CustomWebApplicationFactory<Startup>(_elasticMock);
        }

        [TearDown]
        public void TearDown()
        {
            _factory.Dispose();
        }

        [Test]
        public async Task GetAllBooks_ShouldReturnEmptyList()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync("/Books");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await GetResult<BookGetModel[]>(response);
            result.ShouldBeEmpty();
        }
        
        [Test]
        public async Task AddBook_ShouldReturnAddedItem()
        {
            // Arrange
            var client = _factory.CreateClient();
            var book = new BookUpdateModel
            {
                Author = "Test author",
                Title = "Test title"
            };

            // Act
            var postResponse = await client.PostAsync("/Books", Serialize(book));
            postResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            
            var getResponse = await client.GetAsync("/Books");

            // Assert
            getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await GetResult<BookGetModel[]>(getResponse);
            result.ShouldSatisfyAllConditions(
                () => result.Length.ShouldBe(1),
                () => result[0].Author.ShouldBe("Test author"),
                () => result[0].Title.ShouldBe("Test title"));
        }

        [Test]
        public async Task UpdateBook_ShouldReturnUpdatedItem()
        {
            // Arrange
            var client = _factory.CreateClient();
            var book = new BookUpdateModel
            {
                Author = "Test author",
                Title = "Test title"
            };

            var updatedBook = new BookUpdateModel
            {
                Author = "Test author 2",
                Title = "Test title 2"
            };

            // Act
            var postResponse = await client.PostAsync("/Books", Serialize(book));
            postResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            
            var putResponse = await client.PutAsync("/Books/1", Serialize(updatedBook));
            putResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            
            var getResponse = await client.GetAsync("/Books");

            // Assert
            getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await GetResult<BookGetModel[]>(getResponse);
            result.ShouldSatisfyAllConditions(
                () => result.Length.ShouldBe(1),
                () => result[0].Author.ShouldBe("Test author 2"),
                () => result[0].Title.ShouldBe("Test title 2"));
        }
        
        [Test]
        public async Task DeleteBook_ShouldDeleteItemAndReturnEmptyList()
        {
            // Arrange
            var client = _factory.CreateClient();
            var book = new BookUpdateModel
            {
                Author = "Test author",
                Title = "Test title"
            };

            // Act
            var postResponse = await client.PostAsync("/Books", Serialize(book));
            postResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            
            var getResponse = await client.GetAsync("/Books");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            (await GetResult<BookGetModel[]>(getResponse)).Length.ShouldBe(1);
            
            var deleteResponse = await client.DeleteAsync("/Books/1");
            deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
            
            var getResponse2 = await client.GetAsync("/Books");

            // Assert
            getResponse2.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await GetResult<BookGetModel[]>(getResponse2);
            result.ShouldBeEmpty();
        }
        
        [Test]
        public async Task BulkAddBooks_ShouldReturnAllAddedItems()
        {
            // Arrange
            var client = _factory.CreateClient();

            const string data = @"book_id,goodreads_book_id,best_book_id,work_id,books_count,isbn,isbn13,authors,original_publication_year,original_title,title,language_code,average_rating,ratings_count,work_ratings_count,work_text_reviews_count,ratings_1,ratings_2,ratings_3,ratings_4,ratings_5,image_url,small_image_url
1,2767052,2767052,2792775,272,439023483,9.78043902348e+12,Suzanne Collins,2008.0,The Hunger Games,""The Hunger Games (The Hunger Games, #1)"",eng,4.34,4780653,4942365,155254,66715,127936,560092,1481305,2706317,https://images.gr-assets.com/books/1447303603m/2767052.jpg,https://images.gr-assets.com/books/1447303603s/2767052.jpg
2,3,3,4640799,491,439554934,9.78043955493e+12,""J.K. Rowling, Mary GrandPré"",1997.0,Harry Potter and the Philosopher's Stone,""Harry Potter and the Sorcerer's Stone (Harry Potter, #1)"",eng,4.44,4602479,4800065,75867,75504,101676,455024,1156318,3011543,https://images.gr-assets.com/books/1474154022m/3.jpg,https://images.gr-assets.com/books/1474154022s/3.jpg
3,41865,41865,3212258,226,316015849,9.78031601584e+12,Stephenie Meyer,2005.0,Twilight,""Twilight (Twilight, #1)"",en-US,3.57,3866839,3916824,95009,456191,436802,793319,875073,1355439,https://images.gr-assets.com/books/1361039443m/41865.jpg,https://images.gr-assets.com/books/1361039443s/41865.jpg
4,2657,2657,3275794,487,61120081,9.78006112008e+12,Harper Lee,1960.0,To Kill a Mockingbird,To Kill a Mockingbird,eng,4.25,3198671,3340896,72586,60427,117415,446835,1001952,1714267,https://images.gr-assets.com/books/1361975680m/2657.jpg,https://images.gr-assets.com/books/1361975680s/2657.jpg
5,4671,4671,245494,1356,743273567,9.78074327356e+12,F. Scott Fitzgerald,1925.0,The Great Gatsby,The Great Gatsby,eng,3.89,2683664,2773745,51992,86236,197621,606158,936012,947718,https://images.gr-assets.com/books/1490528560m/4671.jpg,https://images.gr-assets.com/books/1490528560s/4671.jpg";

            // Act
            var postResponse = await client.PostAsync("/Books/BulkAdd?skipHeader=true", Serialize(data));
            postResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            
            var getResponse = await client.GetAsync("/Books");

            // Assert
            getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await GetResult<BookGetModel[]>(getResponse);
            result.ShouldSatisfyAllConditions(
                () => result.Length.ShouldBe(5),
                () => result[0].Author.ShouldBe("Suzanne Collins"),
                () => result[0].Title.ShouldBe("The Hunger Games"),
                () => result[1].Author.ShouldBe("J.K. Rowling, Mary GrandPré"),
                () => result[1].Title.ShouldBe("Harry Potter and the Philosopher's Stone"),
                () => result[2].Author.ShouldBe("Stephenie Meyer"),
                () => result[2].Title.ShouldBe("Twilight"),
                () => result[3].Author.ShouldBe("Harper Lee"),
                () => result[3].Title.ShouldBe("To Kill a Mockingbird"),
                () => result[4].Author.ShouldBe("F. Scott Fitzgerald"),
                () => result[4].Title.ShouldBe("The Great Gatsby"));
        }
        
        [Test]
        public async Task AddBookReview_ShouldAddReview()
        {
            // Arrange
            var client = _factory.CreateClient();
            var book = new BookUpdateModel
            {
                Author = "Test author",
                Title = "Test title"
            };
            
            var review = new BookReviewModel
            {
                Email = "test@example.com",
                Text = "Test text"
            };

            // Act
            var postResponse = await client.PostAsync("/Books", Serialize(book));
            postResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

            var reviewResponse = await client.PostAsync("/Books/1/AddReview", Serialize(review));
            reviewResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            
            var getResponse = await client.GetAsync("/Books");

            // Assert
            getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await GetResult<BookGetModel[]>(getResponse);
            result.ShouldSatisfyAllConditions(
                () => result.Length.ShouldBe(1),
                () => result[0].Author.ShouldBe("Test author"),
                () => result[0].Title.ShouldBe("Test title"),
                () => result[0].Reviews.Count().ShouldBe(1),
                () => result[0].Reviews.ElementAt(0).Email.ShouldBe("test@example.com"),
                () => result[0].Reviews.ElementAt(0).Text.ShouldBe("Test text"));
        }
        
        [Test]
        public async Task SearchBook_ShouldReturnAddedItem()
        {
            // Arrange
            var bookIds = new List<int>();
            
            _elasticMock
                .Setup(m => m.IndexBook(It.IsAny<Book>()))
                .Callback<Book>(b => bookIds.Add(b.Id));
            _elasticMock
                .Setup(m => m.Search(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>()))
                .Returns<string, int, int>((q, s, t) => bookIds.Select(id => new EsBookModel {Id = id}).ToArray());
            
            var client = _factory.CreateClient();
            
            var book = new BookUpdateModel
            {
                Author = "Test author",
                Title = "Test title"
            };

            // Act
            var postResponse = await client.PostAsync("/Books", Serialize(book));
            postResponse.StatusCode.ShouldBe(HttpStatusCode.Created);
            
            var getResponse = await client.GetAsync("/Books/Search/Test");

            // Assert
            getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await GetResult<BookGetModel[]>(getResponse);
            result.ShouldSatisfyAllConditions(
                () => result.Length.ShouldBe(1),
                () => result[0].Author.ShouldBe("Test author"),
                () => result[0].Title.ShouldBe("Test title"));
        }

        private HttpContent Serialize(object obj)
        {
            return new StringContent(
                JsonConvert.SerializeObject(obj), 
                Encoding.UTF8, 
                "application/json");
        }

        private static async Task<T> GetResult<T>(HttpResponseMessage response)
        {
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}