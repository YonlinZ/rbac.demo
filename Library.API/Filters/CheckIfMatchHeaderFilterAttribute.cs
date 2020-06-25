using Library.API.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.API.Filters
{
    /// <summary>
    /// 更新资源
    /// </summary>
    public class CheckIfMatchHeaderFilterAttribute : ActionFilterAttribute
    {
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.HttpContext.Request.Headers.ContainsKey(HeaderNames.IfMatch))
            {
                context.Result = new BadRequestObjectResult(new ApiError { Message = "必须提供 If-Match 消息头"});
            }
            return base.OnActionExecutionAsync(context, next);
        }

    }
}
