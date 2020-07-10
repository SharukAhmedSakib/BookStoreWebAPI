using BookStoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreWebAPI.Services
{
    public class BookRepository : IBookRepository
    {
        private readonly BookDbContext bookContext;

        public BookRepository(BookDbContext bookContext)
        {
            this.bookContext = bookContext;
        }
        public bool BookExists(int bookId)
        {
            return bookContext.Books.Any(b => b.Id == bookId);
        }

        public bool BookExists(string bookIsbn)
        {
            return bookContext.Books.Any(b => b.Isbn == bookIsbn);
        }


        public bool CreateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = bookContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = bookContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            foreach (var author in authors)
            {
                var bookAuthor = new BookAuthor
                {
                    Author = author,
                    Book = book
                };
                bookContext.Add(bookAuthor);
            }

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory
                {
                    Book = book,
                    Category = category
                };
                bookContext.Add(bookCategory);
            }

            bookContext.Add(book);
            return Save();
         }

        public bool DeleteBook(Book book)
        {
            bookContext.Remove(book);
            return Save();
        }

        public Book GetBook(int bookId)
        {
            return bookContext.Books.Where(b => b.Id == bookId).FirstOrDefault();
        }

        public Book GetBook(string bookIsbn)
        {
            return bookContext.Books.Where(b => b.Isbn == bookIsbn).FirstOrDefault();
        }

        public decimal GetBookRating(int bookId)
        {
            var reviews = bookContext.Reviews.Where(r => r.Book.Id == bookId).ToList();

            if (reviews.Count()<=0)
            {
                return 0;
            }

            return ((decimal)reviews.Sum(r => r.Rating) / reviews.Count());
        }

        public ICollection<Book> GetBooks()
        {
            return bookContext.Books.OrderBy(b => b.Title).ToList();
        }

        public bool IsDuplicateIsbn(int bookId, string bookIsbn)
        {
            var book = bookContext.Books.
                Where(b => b.Isbn.Trim().ToUpper() == bookIsbn.Trim().ToUpper() 
                && b.Id != bookId).FirstOrDefault();
            return book == null ? false : true;
        }

        public bool Save()
        {
            var saved = bookContext.SaveChanges();
            return saved >= 0 ? true : false;
        }


        public bool UpdateBook(List<int> authorsId, List<int> categoriesId, Book book)
        {
            var authors = bookContext.Authors.Where(a => authorsId.Contains(a.Id)).ToList();
            var categories = bookContext.Categories.Where(c => categoriesId.Contains(c.Id)).ToList();

            var bookAuthorsToDelete = bookContext.BookAuthors.Where(b => b.BookId == book.Id);
            var bookCategoriesToDelete = bookContext.BookCategories.Where(b => b.BookId == book.Id);

            bookContext.RemoveRange(bookAuthorsToDelete);
            bookContext.RemoveRange(bookCategoriesToDelete);


            foreach (var author in authors)
            {
                bookContext.RemoveRange();
                var bookAuthor = new BookAuthor
                {
                    Author = author,
                    Book = book
                };
                bookContext.Add(bookAuthor);
            }

            foreach (var category in categories)
            {
                var bookCategory = new BookCategory
                {
                    Book = book,
                    Category = category
                };
                bookContext.Add(bookCategory);
            }

            bookContext.Update(book);
            return Save();
        }
    }
}
