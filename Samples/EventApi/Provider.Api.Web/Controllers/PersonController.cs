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
                Name = "foo",
                Id = 1,
                Email = "foo@bar.com",
            };
            // TODO: Need to handle non "application-json" contentType in the serialization proxy.
            return File(person.ToByteArray(), "application/json");
        }
    }
}