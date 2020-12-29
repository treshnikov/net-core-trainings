using DAL.Common;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class BookReviewRepository : AddableRepository<BookReview>, IBookReviewRepository
    {
        public BookReviewRepository(BooksDbContext context) : base(context)
        {
        }

        protected override DbSet<BookReview> DbSet => Context.BookReviews;
    }
}