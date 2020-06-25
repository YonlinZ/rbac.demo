using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Entities;
using Library.API.Filters;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;

namespace Library.API.Controllers
{
    [Route("api/authors/{authorId}/books")]
    [ApiController]
    [ServiceFilter(typeof(CheckAuthorExistActionFilterAttribute))]
    [Authorize]
    public class BookController : ControllerBase
    {
        //public BookController(IBookRepository bookRepository, IAuthorRepository authorRepository)
        //{
        //    AuthorRepository = authorRepository;
        //    BookRepository = bookRepository;
        //}
        //public IAuthorRepository AuthorRepository { get; }
        //public IBookRepository BookRepository { get; }

        public IRepositoryWrapper RepositoryWrapper { get; }
        public IMapper Mapper { get; }
        public IMemoryCache MemoryCache { get; }

        public BookController(IRepositoryWrapper repositoryWrapper, IMapper mapper, IMemoryCache memoryCache)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
            MemoryCache = memoryCache;
        }


        [HttpGet]
        //public ActionResult<List<BookDto>> GetBooks(Guid authorId)
        //{
        //    if (!AuthorRepository.IsAuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }
        //    return BookRepository.GetBooksForAuthor(authorId).ToList();
        //}
        //[Authorize(Roles = "Administrator")]
        //[AllowAnonymous]
        public async Task<ActionResult<IEnumerable<BookDto>>> GetBooksAsync(Guid authorId)
        {
            // 使用内存缓存
            string key = $"{authorId}_books";
            if (!MemoryCache.TryGetValue(key, out List<BookDto> bookDtoList))
            {
                var books = await RepositoryWrapper.Book.GetBooksAsync(authorId);
                bookDtoList = Mapper.Map<IEnumerable<BookDto>>(books).ToList();
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTime.Now.AddMinutes(10);
                options.Priority = CacheItemPriority.Normal;
                MemoryCache.Set(key, bookDtoList, options);
            }
            return bookDtoList.ToList();
        }


        //[HttpGet("{bookId}", Name = nameof(GetBook))]
        //public ActionResult<BookDto> GetBook(Guid authorId, Guid bookId)
        //{
        //    if (!AuthorRepository.IsAuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }
        //    var targetBook = BookRepository.GetBookForAuthor(authorId, bookId);
        //    if (targetBook == null)
        //    {
        //        return NotFound();
        //    }
        //    return targetBook;
        //}
        [HttpGet("{bookId}", Name = nameof(GetBookAsync))]
        public async Task<ActionResult<BookDto>> GetBookAsync(Guid authorId, Guid bookId)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }
            var entityHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityHash;

            var bookDto = Mapper.Map<BookDto>(book);
            return bookDto;
        }



        [HttpPost]
        //public IActionResult AddBook(Guid authorId, BookForCreationDto bookForCreationDto)
        //{
        //    if (!AuthorRepository.IsAuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }
        //    var newBook = new BookDto
        //    {
        //        Id = Guid.NewGuid(),
        //        Title = bookForCreationDto.Title,
        //        Description = bookForCreationDto.Description,
        //        Pages = bookForCreationDto.Pages,
        //        AuthorId = authorId,
        //    };
        //    BookRepository.AddBook(newBook);
        //    return CreatedAtRoute(nameof(GetBook), new { authorId = authorId, bookId = newBook.Id }, newBook);
        //}
        public async Task<IActionResult> AddBookAsync(Guid authorId, BookForCreationDto bookForCreationDto)
        {
            var book = Mapper.Map<Book>(bookForCreationDto);
            book.AuthorId = authorId;
            RepositoryWrapper.Book.Create(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("创建资源Book失败");
            }
            var bookDto = Mapper.Map<BookDto>(book);
            return CreatedAtRoute(nameof(GetBookAsync), new { bookId = bookDto.Id }, bookDto);
        }
        [HttpDelete("{bookID}")]
        //public IActionResult DeleteBook(Guid authorId, Guid bookId)
        //{
        //    if (!AuthorRepository.IsAuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }
        //    var book = BookRepository.GetBookForAuthor(authorId, bookId);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }
        //    BookRepository.DeleteBook(book);
        //    return NoContent();
        //}
        public async Task<IActionResult> DeleteBook(Guid authorId, Guid bookId)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }
            RepositoryWrapper.Book.Delelte(book);
            return NoContent();
        }
        [HttpPut("{bookId}")]
        //public IActionResult UpdateBook(Guid authorId, Guid bookId, BookForUpdateDto updatedBook)
        //{
        //    if (!AuthorRepository.IsAuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }
        //    var book = BookRepository.GetBookForAuthor(authorId, bookId);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }
        //    BookRepository.UpdateBook(authorId, bookId, updatedBook);
        //    return NoContent();
        //}
        [CheckIfMatchHeaderFilter]
        public async Task<IActionResult> UpdateBookAsync(Guid authorId, Guid bookId, BookForUpdateDto updatedBook)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }
            #region 处理并发，通过 ETag 判断资源是否有更改
            var entityHash = HashFactory.GetHash(book);
            if (Request.Headers.TryGetValue(HeaderNames.IfMatch, out var requestETag)
                && requestETag != entityHash)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }
            #endregion
            Mapper.Map(updatedBook, book, typeof(BookForUpdateDto), typeof(Book));
            RepositoryWrapper.Book.Update(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("更新资源Book失败");
            }
            var entityNewHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityNewHash;

            return NoContent();
        }



        [HttpPatch("{bookId}")]
        //public IActionResult PartiallyUpdateBook(Guid authorId, Guid bookId, JsonPatchDocument<BookForUpdateDto> patchDocument)
        //{
        //    if (!AuthorRepository.IsAuthorExists(authorId))
        //    {
        //        return NotFound();
        //    }
        //    var book = BookRepository.GetBookForAuthor(authorId, bookId);
        //    if (book == null)
        //    {
        //        return NotFound();
        //    }
        //    var bookToPatch = new BookForUpdateDto
        //    {
        //        Title = book.Title,
        //        Description = book.Description,
        //        Pages = book.Pages
        //    };
        //    patchDocument.ApplyTo(bookToPatch, ModelState);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    BookRepository.UpdateBook(authorId, bookId, bookToPatch);
        //    return NoContent();
        //}
        [CheckIfMatchHeaderFilter]
        public async Task<IActionResult> PartiallyUpdateBookAsync(Guid authorId, Guid bookId, JsonPatchDocument<BookForUpdateDto> patchDocument)
        {
            var book = await RepositoryWrapper.Book.GetBookAsync(authorId, bookId);
            if (book == null)
            {
                return NotFound();
            }
            #region 处理并发，通过 ETag 判断资源是否有更改
            var entityHash = HashFactory.GetHash(book);
            if (Request.Headers.TryGetValue(HeaderNames.IfMatch, out var requestETag)
                && requestETag != entityHash)
            {
                return StatusCode(StatusCodes.Status412PreconditionFailed);
            }
            #endregion
            var bookUpdateDto = Mapper.Map<BookForUpdateDto>(book);
            patchDocument.ApplyTo(bookUpdateDto, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            Mapper.Map(bookUpdateDto, book, typeof(BookForUpdateDto), typeof(Book));
            RepositoryWrapper.Book.Update(book);
            if (!await RepositoryWrapper.Book.SaveAsync())
            {
                throw new Exception("更新资源Book失败");
            }

            var entityNewHash = HashFactory.GetHash(book);
            Response.Headers[HeaderNames.ETag] = entityNewHash;
            return NoContent();
        }
    }
}
