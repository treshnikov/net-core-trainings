﻿using System.Collections.Generic;

namespace Domain
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public IReadOnlyCollection<BookReview> Reviews { get; set; }
    }
}