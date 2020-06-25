using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using Library.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;

namespace Library.API.Controllers
{
    [Route("api/authors")]
    [ApiController]
    [Authorize]
    public class AuthorController : ControllerBase
    {
        //public IAuthorRepository AuthorRepository { get; }
        public IRepositoryWrapper RepositoryWrapper { get; }
        public IMapper Mapper { get; }
        public ILogger<AuthorController> Logger { get; }

        public AuthorController(IRepositoryWrapper repositoryWrapper, IMapper mapper, ILogger<AuthorController> logger)
        {
            RepositoryWrapper = repositoryWrapper;
            Mapper = mapper;
            Logger = logger;
        }
        //public ActionResult<List<AuthorDto>> GetAuthors()
        //{
        //    return AuthorRepository.GetAuthors().ToList();
        //}
        [HttpGet(Name = nameof(GetAuthorsAsync))]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new string[] { "sortBy", "searchQuery" })]//VaryByQueryKeys属性根据不同的查询关键字来区分不同的响应缓存
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AuthorDto>>> GetAuthorsAsync([FromQuery] AuthorResourceParameters parameters)
        {

            Logger.LogInformation("Hello World, this is logger test");
            var pagedList = await RepositoryWrapper.Author.GetAllAsync(parameters);
            var paginationMetadata = new
            {
                totalCount = pagedList.TotalCount,
                pageSize = pagedList.PageSize,
                currentPage = pagedList.CurrentPage,
                totalPages = pagedList.TotalPages,
                previousePageLink = pagedList.HasPrevious ? Url.Link(nameof(GetAuthorsAsync), new
                {
                    pageNumber = pagedList.CurrentPage - 1,
                    pageSize = pagedList.PageSize,
                    birthPlace = parameters.BirthPlace,
                    serachQuery = parameters.SearchQuery,
                    sortBy = parameters.SortBy,
                }) : null,
                nextPageLink = pagedList.HasNext ? Url.Link(nameof(GetAuthorsAsync), new
                {
                    pageNumber = pagedList.CurrentPage + 1,
                    pageSize = pagedList.PageSize,
                    birthPlace = parameters.BirthPlace,
                    serachQuery = parameters.SearchQuery,
                    sortBy = parameters.SortBy,
                }) : null
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(paginationMetadata));
            var authorDtoList = Mapper.Map<IEnumerable<AuthorDto>>(pagedList);
            return authorDtoList.ToList();
        }

        [HttpGet("{authorId}", Name = nameof(GetAuthorAsync))]
        [ResponseCache(CacheProfileName = "Default")]
        public async Task<ActionResult<AuthorDto>> GetAuthorAsync(Guid authorId)
        {
            var author = await RepositoryWrapper.Author.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }
            #region 缓存，通过 ETag 判断资源是否有更改
            var entityHash = HashFactory.GetHash(author);
            Response.Headers[HeaderNames.ETag] = entityHash;
            if (Request.Headers.TryGetValue(HeaderNames.IfNoneMatch, out var requestETag) && requestETag == entityHash)
            {
                return StatusCode(StatusCodes.Status304NotModified);
            }
            #endregion

            return Mapper.Map<AuthorDto>(author);
        }


        [HttpPost]
        //public IActionResult CreateAuthor(AuthorForCreationDto authorForCreationDto)
        //{
        //    var authorDto = new AuthorDto
        //    {
        //        Name = authorForCreationDto.Name,
        //        Age = authorForCreationDto.Age,
        //        Email = authorForCreationDto.Email
        //    };
        //    AuthorRepository.AddAuthor(authorDto);
        //    return CreatedAtRoute(nameof(GetAuthor), new { authorId = authorDto.Id }, authorDto);
        //}
        public async Task<ActionResult> CreateAuthorAsync(AuthorForCreationDto authorForCreationDto)
        {
            var author = Mapper.Map<Author>(authorForCreationDto);
            RepositoryWrapper.Author.Create(author);
            var result = await RepositoryWrapper.Author.SaveAsync();
            if (!result)
            {
                throw new Exception("创建资源author失败");
            }
            var authorCreated = Mapper.Map<AuthorDto>(author);
            return CreatedAtRoute(nameof(GetAuthorAsync),
                new { authorId = authorCreated.Id },
                authorCreated);
        }



        [HttpDelete("{authorId}")]
        //public IActionResult DeleteAuthor(Guid authorId)
        //{
        //    var author = AuthorRepository.GetAuthor(authorId);
        //    if (author == null)
        //    {
        //        return NotFound();
        //    }
        //    AuthorRepository.DeleteAuthor(author);
        //    return NoContent();
        //}
        public async Task<ActionResult> DeleteAuthorAsync(Guid authorId)
        {
            var author = await RepositoryWrapper.Author.GetByIdAsync(authorId);
            if (author == null)
            {
                return NotFound();
            }
            RepositoryWrapper.Author.Delelte(author);
            var result = await RepositoryWrapper.Author.SaveAsync();
            if (!result)
            {
                throw new Exception("删除资源author失败");
            }
            return NoContent();
        }
    }
}
