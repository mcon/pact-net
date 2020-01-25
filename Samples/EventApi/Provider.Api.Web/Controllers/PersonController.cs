using Contract;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;

namespace Provider.Api.Web.Controllers
{
    [Route("api")]
    [ApiController]
    public class PerconController : ControllerBase
    {
        // GET api/values
        [HttpGet("values")]
        public ActionResult<byte[]> Get()
        {
            var person = new Person
            {
                name = "Foo",
                id = 1,
                email = "foo@bar.com",
            };
            return File(person.ToByteArray(), "application/json"); // TODO: Support non-application/json headers
        }
    }
}