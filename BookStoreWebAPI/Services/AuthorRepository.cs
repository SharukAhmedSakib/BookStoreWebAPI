using BookStoreWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStoreWebAPI.Services
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly BookDbContext authorContext;

        public AuthorRepository(BookDbContext authorContext)
        {
            this.authorContext = authorContext;
        }
        public bool AuthorExists(int authorId)
        {
            return authorContext.Authors.Any(a => a.Id == authorId);
        }

        public bool CreateAuthor(Author author)
        {
            authorContext.Add(author);
            return Save();
        }

        public bool DeleteAuthor(Author author)
        {
            authorContext.Remove(author);
            return Save();
        }

        public Author GetAuthor(int authorId)
        {
            return authorContext.Authors.Where(a => a.Id == authorId).FirstOrDefault();
        }

        public ICollection<Author> GetAuthors()
        {
            return authorContext.Authors.OrderBy(a => a.LastName).ToList();
        }

        public ICollection<Author> GetAuthorsOfABook(int bookId)
        {
            return authorContext.BookAuthors.Where(ba => ba.BookId == bookId).Select(a => a.Author).ToList();

        }

        public ICollection<Book> GethBooksByAuthor(int authorId)
        {
            return authorContext.BookAuthors.Where(ba => ba.AuthorId == authorId).Select(b => b.Book).ToList();
        }

        public bool Save()
        {
            var saved = authorContext.SaveChanges();
            return saved >= 0 ? true : false;
        }

        public bool UpdateAuthor(Author author)
        {
            authorContext.Update(author);
            return Save();
        }
    }
}
