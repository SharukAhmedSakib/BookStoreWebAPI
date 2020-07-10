using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookStoreWebAPI.Dtos;
using BookStoreWebAPI.Models;
using BookStoreWebAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreWebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IBookRepository bookRepository;
        private readonly ICountryRepository countryRepository;

        public AuthorsController(IAuthorRepository authorRepository, IBookRepository bookRepository, ICountryRepository countryRepository)
        {
            this.authorRepository = authorRepository;
            this.bookRepository = bookRepository;
            this.countryRepository = countryRepository;
        }


        //api/authors
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        [ProducesResponseType(400)]
        public IActionResult GetAuthors()
        {

            var authors = authorRepository.GetAuthors();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorDtos = new List<AuthorDto>();
            foreach (var author in authors)
            {
                authorDtos.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorDtos);
        }


        //api/authors/authorId
        [HttpGet("{authorId}", Name ="GetAuthor")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(AuthorDto))]
        public IActionResult GetAuthor(int authorId)
        {
            if (!authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var author = authorRepository.GetAuthor(authorId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorDto = new AuthorDto
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName
            };

            return Ok(authorDto);
        }


        //api/authors/authorId/books
        [HttpGet("{authorId}/books")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookDto>))]
        public IActionResult GetBooksByAuthor(int authorId)
        {
            if (!authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var books = authorRepository.GethBooksByAuthor(authorId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bookDtos = new List<BookDto>();

            foreach (var book in books)
            {
                bookDtos.Add(new BookDto
                {
                    Id = book.Id,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    DatePublished = book.DatePublished
                });
            }
            return Ok(bookDtos);
        }


        //api/authors/bookId/authors
        [HttpGet("{bookId}/authors")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200, Type = typeof(IEnumerable<AuthorDto>))]
        public IActionResult GetAuthorsOfABook(int bookId)
        {
            if (!bookRepository.BookExists(bookId))
            {
                return NotFound();
            }

            var authors = authorRepository.GetAuthorsOfABook(bookId);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authorDtos = new List<AuthorDto>();
            foreach (var author in authors)
            {
                authorDtos.Add(new AuthorDto
                {
                    Id = author.Id,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                });
            }

            return Ok(authorDtos);
        }

        //api/authors
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(Author))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [ProducesResponseType(500)]
        public IActionResult CreateAuthor([FromBody] Author authorToCreate)
        {
            if (authorToCreate == null)
            {
                return BadRequest(ModelState);
            }

            var author = authorRepository.GetAuthors()
                .Where(a => a.LastName.Trim().ToUpper() == authorToCreate.LastName.Trim().ToUpper()
                && a.FirstName.Trim().ToUpper() == authorToCreate.FirstName.Trim().ToUpper())
                .FirstOrDefault();

            if (author != null)
            {
                ModelState.AddModelError("", $"{authorToCreate.FirstName} {authorToCreate.FirstName} already exists");
                return StatusCode(422, ModelState);
            }

            if (!countryRepository.CountryExists(authorToCreate.Country.Id))
            {
                ModelState.AddModelError("", "Country doesn't exist!");
            }

            authorToCreate.Country = countryRepository.GetCountry(authorToCreate.Country.Id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!authorRepository.CreateAuthor(authorToCreate))
            {
                ModelState.AddModelError("", $"Something went wrong creating new author");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetAuthor", new { authorId = authorToCreate.Id }, authorToCreate);
        }



        //api/authors/authorId 
        [HttpPut("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult UpdateAuthor(int authorId, [FromBody]Author updatedAuthor)
        {
            if (updatedAuthor == null)
            {
                return BadRequest(ModelState);
            }

            if (authorId != updatedAuthor.Id)
            {
                return BadRequest(ModelState);
            }

            if (!authorRepository.AuthorExists(updatedAuthor.Id))
            {
                ModelState.AddModelError("", "Author doesn't exist!");
            }

            if (!countryRepository.CountryExists(updatedAuthor.Country.Id))
            {
                ModelState.AddModelError("", "Country doesn't exist!");
            }

            if (!ModelState.IsValid)
            {
                return StatusCode(404, ModelState);
            }

            updatedAuthor.Country = countryRepository.GetCountry(updatedAuthor.Country.Id);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!authorRepository.UpdateAuthor(updatedAuthor))
            {
                ModelState.AddModelError("", $"Something went wrong updating the Auhtor Info!");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


        //api/authors/authorId
        [HttpDelete("{authorId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public IActionResult DeleteAuthor(int authorId)
        {
            if (!authorRepository.AuthorExists(authorId))
            {
                return NotFound();
            }

            var authorToDelete = authorRepository.GetAuthor(authorId);

            if (authorRepository.GethBooksByAuthor(authorId).Count() > 0)
            {
                ModelState.AddModelError("", $"{authorToDelete.FirstName} {authorToDelete.LastName}" + " cannot be deleted because he is associated with at least one book");
                return StatusCode(409, ModelState);
            }


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!authorRepository.DeleteAuthor(authorToDelete))
            {
                ModelState.AddModelError("", $"Something went wrong deleting {authorToDelete.FirstName} {authorToDelete.LastName}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }


    }
}