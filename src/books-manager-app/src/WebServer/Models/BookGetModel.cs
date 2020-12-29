using System.Collections.Generic;

namespace WebServer.Models
{
    public class BookGetModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public IEnumerable<BookReviewModel> Reviews { get; set; }
    }
}