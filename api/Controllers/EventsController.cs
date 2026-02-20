using EventsApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace EventsApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private static readonly List<Event> _events = new()
    {
        new Event
        {
            Id = 1,
            Title = "Tech Conference 2025",
            Date = "2025-06-15",
            Location = "Seattle, WA",
            Description = "Annual technology conference featuring the latest in software development, cloud computing, and AI.",
            AvailableTickets = 150,
            Price = 299
        },
        new Event
        {
            Id = 2,
            Title = "React Summit",
            Date = "2025-07-20",
            Location = "New York, NY",
            Description = "A deep-dive into modern React patterns, hooks, and performance optimization techniques.",
            AvailableTickets = 80,
            Price = 199
        },
        new Event
        {
            Id = 3,
            Title = "DevOps Days",
            Date = "2025-08-05",
            Location = "Austin, TX",
            Description = "Explore CI/CD pipelines, containerization, and infrastructure-as-code best practices.",
            AvailableTickets = 200,
            Price = 149
        }
    };

    // GET api/events
    [HttpGet]
    public ActionResult<IEnumerable<Event>> GetEvents()
    {
        return Ok(_events);
    }

    // GET api/events/{id}
    [HttpGet("{id}")]
    public ActionResult<Event> GetEvent(int id)
    {
        var ev = _events.FirstOrDefault(e => e.Id == id);
        if (ev is null)
            return NotFound();
        return Ok(ev);
    }
}
