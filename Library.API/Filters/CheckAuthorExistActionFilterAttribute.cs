using Library.API.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Filters
{
    public class CheckAuthorExistActionFilterAttribute : ActionFilterAttribute
    {
        public CheckAuthorExistActionFilterAttribute(IRepositoryWrapper repositoryWrapper)
        {
            RepositoryWrapper = repositoryWrapper;
        }

        public IRepositoryWrapper RepositoryWrapper { get; }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var authorIdParameter = context.ActionArguments.Single(m => m.Key == "authorId");
            var authorId = (Guid)authorIdParameter.Value;
            var isExist = await RepositoryWrapper.Author.IsExistAsync(authorId);
            if (!isExist)
            {
                context.Result = new NotFoundResult();
            }
            await base.OnActionExecutionAsync(context, next);
        }
    }
}
