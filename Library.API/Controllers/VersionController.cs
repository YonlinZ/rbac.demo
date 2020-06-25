using Microsoft.AspNetCore.Mvc;
namespace Library.API.Controllers.V1
{
    [Route("api/person")]
    [ApiVersion("1.0")]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get() => "Result from v1";
    }
}
namespace Library.API.Controllers.V2
{
    [Route("api/person2")]
    [ApiVersion("2.0")]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get() => "Result from v2";
    }
}
// 这种方式并不支持隐式版本匹配
namespace Library.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/students")]
    public class StudentController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get() => "Result from v1";
    }
}
namespace Library.API.Controllers.V2
{
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/students2")]
    public class StudentController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get() => "Result from v2";
    }
}

//namespace Library.API.Controllers
//{
//    [Route("api/news")]
//    [ApiVersion("1.0")]
//    [ApiVersion("2.0")]
//    public class NewsController : ControllerBase
//    {
//        [HttpGet]
//        public ActionResult<string> Get() => "Result from v1";
//        [HttpGet, MapToApiVersion("2.0")]
//        public ActionResult<string> GetV2() => "Result from v2";
//    }

//    [ApiVersion("2.0")]
//    [ApiVersion("1.0", Deprecated = true)]
//    [Route("api/[controller]")]
//    public class HelloWorldController : Controller
//    {
//        [HttpGet]
//        public string Get() => "Hello world!";
//        [HttpGet, MapToApiVersion("2.0")]
//        public string GetV2() => "Hello world v2.0!";
//    }
//}