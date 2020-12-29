using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.BookManager;
using Domain;
using Microsoft.AspNetCore.Mvc;
using BusinessLogic.Exceptions;
using WebServer.Models;

namespace WebServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IBooksManager _booksManager;

        public BooksController(IBooksManager booksManager)
        {
            _booksManager = booksManager;
        }
        
        [HttpGet]
        public IActionResult Get(int skip = 0, int take = 20)
        {
            if (skip < 0 || take <= 0)
            {
                return BadRequest();
            }

            if (take > 50)
            {
                return BadRequest("\"take\" should be not greater than 50");
            }
            
            return Ok(_booksManager.Get(skip, take)
                .Select(ToModel));
        }
        
        [HttpGet]
        [Route("{id}")]
        public IActionResult Get(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            var book = _booksManager.Get(id);
            return Ok(ToModel(book));
        }

        [HttpPost]
        public IActionResult Add([FromBody] BookUpdateModel model)
        {
            ValidateBook(model);

            var book = FromModel(model);
            _booksManager.Add(book);

            return CreatedAtAction(nameof(Get), book.Id, ToModel(book));
        }

        [HttpPut]
        [Route("{id}")]
        public IActionResult Update(int id, [FromBody] BookUpdateModel model)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            ValidateBook(model);

            var book = FromModel(model);
            book.Id = id;
            _booksManager.Update(book);

            return NoContent();
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }
            
            _booksManager.Delete(id);

            return NoContent();
        }
        
        [HttpPost]
        [Route("BulkAdd")]
        public async Task<IActionResult> BulkAdd()
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8);
            var data = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(data))
            {
                return BadRequest();
            }

            var result = _booksManager.BulkAdd(true, data);

            return Ok(result);
        }

        [HttpGet]
        [Route("Search/{query}")]
        public IActionResult Search(string query, int skip = 0, int take = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return BadRequest("Empty query");
            }

            if (skip < 0 || take <= 0)
            {
                return BadRequest();
            }

            if (take > 50)
            {
                return BadRequest("\"take\" should be not greater than 50");
            }
            
            var result = _booksManager.Search(query, skip, take);
            return Ok(result.Select(ToModel));
        }

        private void ValidateBook(BookUpdateModel model)
        {
            if (model == null)
            {
                throw new BadRequestException();
            }

            if (string.IsNullOrWhiteSpace(model.Title))
            {
                throw new BadRequestException("Title should not be empty");
            }

            if (string.IsNullOrWhiteSpace(model.Author))
            {
                throw new BadRequestException("Author should not be empty");
            }
        }
        
        private Book FromModel(BookUpdateModel model)
        {
            return new Book
            {
                Title = model.Title,
                Author = model.Author
            };
        }
        
        private BookGetModel ToModel(Book book)
        {
            return new BookGetModel
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Reviews = book.Reviews.Select(BookReviewController.ToModel)
            };
        }
    }
}