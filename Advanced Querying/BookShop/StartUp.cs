namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Globalization;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            // DbInitializer.ResetDatabase(db);
            ;
            //Console.WriteLine(GetBooksByAgeRestriction(db, input));
            //Console.WriteLine(GetGoldenBooks(db));
            //Console.WriteLine(GetBooksByPrice(db));
            //Console.WriteLine(GetBooksNotReleasedIn(db, year));
            //Console.WriteLine(GetBooksReleasedBefore(db, input));
            //Console.WriteLine(GetBooksReleasedBefore(db, date));
            //Console.WriteLine(GetAuthorNamesEndingIn(db, str));
            //Console.WriteLine(GetBookTitlesContaining(db, str));
            //Console.WriteLine(GetBooksByAuthor(db, str));
            //Console.WriteLine(CountBooks(db, num));
            //Console.WriteLine(CountCopiesByAuthor(db));
            //Console.WriteLine(GetTotalProfitByCategory(db));
            //Console.WriteLine(GetMostRecentBooks(db));
            //IncreasePrices(db);
            //Console.WriteLine(RemoveBooks(db));

        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command) {

            if (!Enum.TryParse<AgeRestriction>(command, true, out var ageRestriction))
            {
                return $"{command} is not a valid age restriction";
            }

            var books = context.Books.Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => new {
                    b.Title
                })
                .OrderBy(b => b.Title)
                .ToList();


            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetGoldenBooks(BookShopContext context) {
            string goldType = "Gold";
            if (!Enum.TryParse<EditionType>(goldType, false, out var editionType))
            {
                return $"There was an error";
            }
            var books = context.Books.Where(b => b.EditionType == editionType && b.Copies < 5000)
                .Select(b => new
                {
                    b.Title,
                    b.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByPrice(BookShopContext context) {
            var book = context.Books.Where(b => b.Price > 40).Select(b => new
            {
                b.Title,
                b.Price
            })
                .OrderByDescending(b => b.Price)
                .ToList();

            return String.Join(Environment.NewLine, book.Select(b => $"{b.Title} - {b.Price:C2}"));
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year) {
            var books = context.Books.Where(b => b.ReleaseDate.Value.Year != year)
                .Select(b => new {
                    b.Title,
                    b.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByCategory(BookShopContext context, string input) {
            string[] category = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(c => c.ToLower()).ToArray();

            var books = context.Books
                .Select(b => new {
                    b.Title,
                    b.BookCategories
                }).Where(b => b.BookCategories.Any(bc => category.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date) {
            var parseDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .Where(b => b.ReleaseDate < parseDate)
                .Select(b => new {
                    b.Title,
                    b.EditionType,
                    b.Price,
                    b.ReleaseDate
                })
                .OrderByDescending(b => b.ReleaseDate)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} - {b.EditionType} - {b.Price:c2}"));
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input) {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new { FullName = a.FirstName + " " + a.LastName })
                .OrderBy(a => a.FullName)
                .ToList();

            return string.Join(Environment.NewLine, authors.Select(a => a.FullName));
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input) {
            var books = context.Books
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .Select(b => new { b.Title })
                .OrderBy(b => b.Title)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => b.Title));
        }

        public static string GetBooksByAuthor(BookShopContext context, string input) {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(b => new {
                    b.Title,
                    AuthorFullName = b.Author.FirstName + " " + b.Author.LastName,
                    b.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.Title} ({b.AuthorFullName})"));
        }

        public static int CountBooks(BookShopContext context, int lengthCheck) {
            var books = context.Books.Count(b => b.Title.Length > lengthCheck);
            return books;
        }

        public static string CountCopiesByAuthor(BookShopContext context) {
            var books = context.Authors
                .Select(a => new
                {
                    AuthorFullName = a.FirstName + " " + a.LastName,
                    BookCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.BookCopies)
                .ToList();

            return string.Join(Environment.NewLine, books.Select(b => $"{b.AuthorFullName} - {b.BookCopies}"));
        }

        public static string GetTotalProfitByCategory(BookShopContext context) {
            var categories = context.Categories
                .Select(c => new {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Sum(x => x.Book.Copies * x.Book.Price)
                })
                .OrderByDescending(x => x.TotalProfit)
                .ThenBy(x => x.Name)
                .ToList();

            return string.Join(Environment.NewLine, categories.Select(c => $"{c.Name} ${c.TotalProfit:f2}"));
        }

        public static string GetMostRecentBooks(BookShopContext context) {
            var categories = context.Categories
                .Select(c => new
                {
                    c.Name,
                    Books = c.CategoryBooks.Select(x => new
                    {
                        x.Book.ReleaseDate,
                        x.Book.Title
                    })
                .OrderByDescending(b => b.ReleaseDate).Take(3)
                }).OrderBy(c => c.Name).ToList();

            StringBuilder sb = new StringBuilder();
            foreach (var book in categories)
            {
                sb.AppendLine($"--{book.Name}");
                foreach (var book2 in book.Books) {
                    sb.AppendLine($"{book2.Title} ({book2.ReleaseDate.Value.Year})");
                }
            }

            return sb.ToString().Trim();
        }

        public static void IncreasePrices(BookShopContext context) {
            var books = context.Books.AsTracking()
                .Where(b => b.ReleaseDate.Value.Year < 2010);

            foreach (var book in books)
            {
                book.Price += 5;
            }
            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context) {
            var books = context.Books.Where(b => b.Copies < 4200);
            int booksToDelete = books.Count();
            context.Books.RemoveRange(books);
            context.SaveChanges();
            return booksToDelete;
        }
    }
}


