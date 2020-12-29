using BusinessLogic;
using BusinessLogic.BookManager;
using BusinessLogic.Exceptions;
using Domain;
using Microsoft.AspNetCore.Mvc;
using WebServer.Models;

namespace WebServer.Controllers
{
    [ApiController]
    [Route("Books")]
    public class BookReviewController : ControllerBase
    {
        private readonly IBooksManager _booksManager;

        public BookReviewController(IBooksManager booksManager)
        {
            _booksManager = booksManager;
        }
        
        [HttpPost]
        [Route("{bookId}/AddReview")]
        public IActionResult AddReview(int bookId, [FromBody] BookReviewModel model)
        {
            if (bookId <= 0)
            {
                return BadRequest();
            }

            ValidateBookReview(model);
            
            _booksManager.AddReview(bookId, FromModel(model));

            return Ok();
        }

        private void ValidateBookReview(BookReviewModel model)
        {
            if (model == null)
            {
                throw new BadRequestException();
            }

            if (string.IsNullOrWhiteSpace(model.Email))
            {
                throw new BadRequestException("Email should not be empty");
            }

            if (!model.Email.Contains("@"))
            {
                throw new BadRequestException("Invalid email format");
            }

            if (string.IsNullOrWhiteSpace(model.Text))
            {
                throw new BadRequestException("Text should not be empty");
            }
        }

        public static BookReview FromModel(BookReviewModel model)
        {
            return new BookReview
            {
                Email = model.Email,
                Text = model.Text
            };
        }

        public static BookReviewModel ToModel(BookReview bookReview)
        {
            return new BookReviewModel
            {
                Email = bookReview.Email,
                Text = bookReview.Text
            };
        }
    }
}