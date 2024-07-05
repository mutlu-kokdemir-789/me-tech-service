using BookStore.Data;
using BookStore.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Reflection.Metadata.BlobBuilder;

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
            BookListResponse bookListResponse = new BookListResponse();

            if (query == null) 
            {
                var books = await _storeDbContext.Books.ToListAsync();
                bookListResponse.NumberOfBooks = books.Count;
                bookListResponse.Books = books;
                return Ok(books);
            }

            bookListResponse = GenerateBookListResponseAccordingToQuery(query);
            return Ok(bookListResponse);
        }

        private BookListResponse GenerateBookListResponseAccordingToQuery(QueryParamsForBookListRequest query)
        {
            BookListResponse bookListResponse = new BookListResponse();
            IQueryable<Book> booksQueryable;

            if (query.Filter != null)
            {
                booksQueryable = GenerateFilteredQueryable(query);
            }
            else
            {
                booksQueryable = _storeDbContext.Books;
            }
            var sortedList = GenerateSortedList(booksQueryable, query.Sort);
            bookListResponse.NumberOfBooks = sortedList.Count;
            List<Book> bookList;
            if (query.PageNumber.HasValue)
            {
                bookList = sortedList.Skip((query.PageNumber.Value - 1) * 20).Take(20).ToList();
            }
            else
            {
                bookList = sortedList.Skip(0).Take(20).ToList();
            }
            bookListResponse.Books = bookList;
            return bookListResponse;
        }


        private IQueryable<Book> GenerateFilteredQueryable(QueryParamsForBookListRequest query)
        {
            var maxPrice = query.Filter.PriceMax;
            var minPrice = query.Filter.PriceMin;
            var minRate = query.Filter.RateMin;
            var maxRate = query.Filter.RateMax;

            IQueryable<Book> booksWQ = _storeDbContext.Books;

            if (maxPrice.HasValue)
            {
                booksWQ = _storeDbContext.Books.Where(book => book.Price < maxPrice);
            }
            if (minPrice.HasValue)
            {
                booksWQ = booksWQ.Where(book => book.Price > minPrice);
            }
            if (minRate.HasValue)
            {
                booksWQ = booksWQ.Where(book => book.Rate > minRate);
            }
            if (maxRate.HasValue)
            {
                booksWQ = booksWQ.Where(book => book.Rate < maxRate);
            }
            return booksWQ;
        }


        private List<Book> GenerateSortedList(IQueryable<Book> booksWQ, string sortStr)
        {
            IQueryable<Book> bookWQLocal = booksWQ;
            if (sortStr != null)
            {
                IOrderedEnumerable<Book> bookIOE;
                if (sortStr.Equals("Price - Asc"))
                {
                    bookWQLocal = booksWQ.OrderBy(book => book.Price.Value);
                }
                else if (sortStr.Equals("Price - Desc"))
                {
                    bookWQLocal = booksWQ.OrderByDescending(book => book.Price.Value);
                }
                else if (sortStr.Equals("Rate - Asc"))
                {
                    bookWQLocal = booksWQ.OrderBy(book => book.Rate.Value);
                }
                else if (sortStr.Equals("Rate - Desc"))
                {
                    bookWQLocal = booksWQ.OrderByDescending(book => book.Rate.Value);
                }
                else if (sortStr.Equals("Publishing Year - Asc"))
                {
                    bookWQLocal = booksWQ.OrderBy(book => book.PublishingYear.Value);
                }
                else // publishing year descending
                {
                    bookWQLocal = booksWQ.OrderByDescending(book => book.PublishingYear.Value);
                }
            }
            return bookWQLocal.ToList();
        }

        [HttpDelete("Id"), Authorize]
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
            await _storeDbContext.SaveChangesAsync();
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


        [HttpPost("CommentAndRate"), Authorize]
        public async Task<ActionResult<int>> CommentAndRate(CommentAndRateRequestData commentAndRateRequestData)
        {
            if (commentAndRateRequestData == null) 
            {
                return BadRequest();
            }

            if (commentAndRateRequestData.Rate > 0)
            {
                var rate = await _storeDbContext.Rates.FirstOrDefaultAsync(rate => 
                    rate.UserId.ToString() == commentAndRateRequestData.UserId.ToString() & 
                    rate.BookId.ToString() == commentAndRateRequestData.BookId.ToString()
                    );
                if (rate != null) 
                {
                    rate.Value = commentAndRateRequestData.Rate;
                } 
                else
                {
                    _storeDbContext.Rates.Add(new Rate
                    {
                        Id = Guid.NewGuid(),
                        UserId = commentAndRateRequestData.UserId,
                        BookId = commentAndRateRequestData.BookId,
                        Value = commentAndRateRequestData.Rate
                    });
                    await _storeDbContext.SaveChangesAsync();
                }
                var ratesOfTheBook = _storeDbContext.Rates.Where(rate => rate.BookId.ToString() == commentAndRateRequestData.BookId.ToString()).ToList();
                var average = ratesOfTheBook.Average(rate => rate.Value);
                var theBook = await _storeDbContext.Books.FindAsync(commentAndRateRequestData.BookId);
                if (theBook != null)
                {
                    theBook.Rate = average;
                }
                await _storeDbContext.SaveChangesAsync();
            }

            if (commentAndRateRequestData.Comment != String.Empty) 
            {
                _storeDbContext.Comments.Add(new Comment
                {
                    Id = Guid.NewGuid(),
                    UserId = commentAndRateRequestData.UserId,
                    BookId = commentAndRateRequestData.BookId,
                    Message = commentAndRateRequestData.Comment
                });
                await _storeDbContext.SaveChangesAsync();
            }

            return Ok();
        }


        [HttpPatch("Update"), Authorize]
        public async Task<ActionResult<Book>> Update([FromBody] Book book)
        {
            var bookFromDb = await _storeDbContext.Books.FindAsync(new Guid(book.Id.ToString()));
            if (bookFromDb != null)
            {
                if (book.Author != null)
                {
                    bookFromDb.Author = book.Author;
                }

                if (book.PublishingYear != null)
                {
                    bookFromDb.PublishingYear = book.PublishingYear;
                }

                if (book.Title != null)
                {
                    bookFromDb.Title = book.Title;
                }

                if (book.Price != null)
                {
                    bookFromDb.Price = book.Price;
                }
            }
            await _storeDbContext.SaveChangesAsync();
            return Ok(bookFromDb);
        }
    }
}
