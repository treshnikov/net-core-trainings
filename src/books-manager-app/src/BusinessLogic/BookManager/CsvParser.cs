using Domain;
using TinyCsvParser.Mapping;

namespace BusinessLogic.BookManager
{
    public class CsvBookMapping : CsvMapping<Book>
    {
        // All csv field indexes are here for possible future usage
        private const int FieldBookId = 0;
        private const int FieldGoodreadsBookId = 1;
        private const int FieldBestBookId = 2;
        private const int FieldWorkId = 3;
        private const int FieldBooksCount = 4;
        private const int FieldIsbn = 5;
        private const int FieldIsbn13 = 6;
        private const int FieldAuthors = 7;
        private const int FieldOriginalPublicationYear = 8;
        private const int FieldOriginalTitle = 9;
        private const int FieldTitle = 10;
        private const int FieldLanguageCode = 11;
        private const int FieldAverageRating = 12;
        private const int FieldRatingsCount = 13;
        private const int FieldWorkRatingsCount = 14;
        private const int FieldWorkTextReviewsCount = 15;
        private const int FieldRatings1 = 16;
        private const int FieldRatings2 = 17;
        private const int FieldRatings3 = 18;
        private const int FieldRatings4 = 19;
        private const int FieldRatings5 = 20;
        private const int FieldImageUrl = 21;
        private const int FieldSmallImageUrl = 22; 
        
        public CsvBookMapping()
        {
            MapProperty(FieldOriginalTitle, x => x.Title);
            MapProperty(FieldAuthors, x => x.Author);
        }
    }
}