using Library.API.Data;
using Library.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Services
{
    // TODO 没用了
    //public class AuthorMockRepository : IAuthorRepository
    //{
    //    public void AddAuthor(AuthorDto author)
    //    {
    //        author.Id = Guid.NewGuid();
    //        LibraryMockData.Current.Authors.Add(author);
    //    }

    //    public AuthorDto GetAuthor(Guid authorId)
    //    {
    //        var author = LibraryMockData.Current.Authors.FirstOrDefault(au => au.Id == authorId);
    //        return author;
    //    }
    //    public IEnumerable<AuthorDto> GetAuthors()
    //    {
    //        return LibraryMockData.Current.Authors;
    //    }
    //    public bool IsAuthorExists(Guid authorId)
    //    {
    //        return LibraryMockData.Current.Authors.Any(au => au.Id == authorId);
    //    }
    //    public void DeleteAuthor(AuthorDto author)
    //    {
    //        LibraryMockData.Current.Books.RemoveAll(book => book.AuthorId == author.Id);
    //        LibraryMockData.Current.Authors.Remove(author);
    //    }
    //}
}
