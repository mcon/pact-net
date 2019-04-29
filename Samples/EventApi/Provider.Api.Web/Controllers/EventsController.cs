using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Provider.Api.Web.Models;

namespace Provider.Api.Web.Controllers
{
    public class EventsController : ControllerBase
    {
        [HttpGet("events")]
        public IActionResult Get([FromHeader]string authorization, [FromQuery]string type)
        {
            if (!string.IsNullOrEmpty(type))
            {
                return Ok(GetAllEventsFromRepo().Where(x => x.EventType.Equals(type, StringComparison.InvariantCultureIgnoreCase)));
            }
            if (string.IsNullOrEmpty(authorization))
            {
                return new JsonResult(new {Message = "Authorization has been denied for this request."}){StatusCode = 401};
            }
            return Ok(GetAllEventsFromRepo());
        }

        [HttpGet("events/{type}")]
        public object GetByType(string type)
        {
            if (Guid.TryParse(type, out var id))
            {
                return GetAllEventsFromRepo().First(x => x.EventId == id);
            }
            return GetAllEventsFromRepo().Where(x => x.EventType.Equals(type, StringComparison.InvariantCultureIgnoreCase));
        }

        [HttpPost("events")]
        public IActionResult Post(Event @event)
        {
            if (@event == null)
            {
                return new BadRequestResult();
            }

            return new CreatedResult($"events/{@event.EventId}", @event);
        }

        private IEnumerable<Event> GetAllEventsFromRepo()
        {
            return new List<Event>
            {
                new Event
                {
                    EventId = Guid.Parse("45D80D13-D5A2-48D7-8353-CBB4C0EAABF5"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:41.0660548"),
                    EventType = "SearchView"
                },
                new Event
                {
                    EventId = Guid.Parse("83F9262F-28F1-4703-AB1A-8CFD9E8249C9"),
                    Timestamp = DateTime.Parse("2014-06-30T01:37:52.2618864"),
                    EventType = "DetailsView"
                },
                new Event
                {
                    EventId = Guid.Parse("3E83A96B-2A0C-49B1-9959-26DF23F83AEB"),
                    Timestamp = DateTime.Parse("2014-06-30T01:38:00.8518952"),
                    EventType = "SearchView"
                }
            };
        }
    }
}