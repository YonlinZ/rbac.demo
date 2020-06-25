using Library.API.Entities;
using Library.API.Helpers;
using Library.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    public interface IAuthorRepository : IRepositoryBase<Author>, IRepositoryBase2<Author, Guid>
    {
        //IEnumerable<AuthorDto> GetAuthors();
        //AuthorDto GetAuthor(Guid authorId);
        //bool IsAuthorExists(Guid authorId);
        //void AddAuthor(AuthorDto author);
        //void DeleteAuthor(AuthorDto author);
        Task<PagedList<Author>> GetAllAsync(AuthorResourceParameters parameters);
    }
}
