using BookStore.Data;
using BookStore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;

namespace BookStore.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoreDbContext _storeDbContext;

        // --
        private Random random = new Random();
        // --

        public BooksController(BookStoreDbContext storeDbContext)
        {
            _storeDbContext = storeDbContext;
        }


        [HttpPost]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks([FromBody] QueryParamsForBookListRequest query)
        {
            if (_storeDbContext.Books == null)
            {
                return NotFound();
            }
            List<Book> books = await _storeDbContext.Books.ToListAsync(); // get them with query **
            BookListResponse bookListResponse = new BookListResponse();
            bookListResponse.NumberOfBooks = books.Count;

            return Ok(bookListResponse);
        }

        [HttpDelete("Id")]
        public async Task<ActionResult<Book>> DeleteBook(string id)
        {
            if (_storeDbContext.Books == null)
            {
                return NotFound();
            }
            var book = await _storeDbContext.Books.FindAsync(new Guid(id));
            if (book == null) 
            {
                return NotFound();
            }
            _storeDbContext.Books.Remove(book);
            _storeDbContext.SaveChanges();
            return Ok();
        }

        [HttpGet("Id")]
        public async Task<ActionResult<Book>> GetBookDetails(string id)
        {
            if (_storeDbContext.Books == null)
            {
                return NotFound();
            }
            var book = await _storeDbContext.Books.FindAsync(new Guid(id));
            if (book == null)
            {
                return NotFound();
            }
            var comments = from c in _storeDbContext.Comments
                           where c.BookId == book.Id
                           join u in _storeDbContext.Users on c.UserId equals u.Id
                           select new CommentResponseForBookDetails
                           {
                               Id = c.Id,
                               UserName = u.Name,
                               Message = c.Message
                           };
            BookDetailsResponse bookDetailsResponse = new BookDetailsResponse();
            bookDetailsResponse.Book = book;
            bookDetailsResponse.Comments = comments.ToList();
            return Ok(bookDetailsResponse);
        }


        [HttpPost("Add"), Authorize]
        public async Task<ActionResult<Book>> AddBook(Book book)
        {
            Guid id = Guid.NewGuid();
            Book bookToBeAdded = new Book();
            bookToBeAdded.Id = id;
            bookToBeAdded.Title = book.Title;
            bookToBeAdded.Author = book.Author;
            bookToBeAdded.PublishingYear = book.PublishingYear;
            bookToBeAdded.Price = book.Price;
            _storeDbContext.Books.Add(bookToBeAdded);
            await _storeDbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
