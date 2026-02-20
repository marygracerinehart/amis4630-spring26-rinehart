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
            Title = "Ohio State Buckeyes Football vs. Penn State",
            Date = "2026-10-24",
            Location = "Ohio Stadium, Columbus, OH",
            Description = "Big Ten showdown at The Horseshoe featuring Ohio State football under the lights.",
            AvailableTickets = 120,
            Price = 145
        },
        new Event
        {
            Id = 2,
            Title = "Ohio State Buckeyes Men's Basketball vs. Michigan",
            Date = "2026-02-14",
            Location = "Value City Arena, Columbus, OH",
            Description = "Rivalry game in Columbus with conference implications and high-energy crowd support.",
            AvailableTickets = 95,
            Price = 85
        },
        new Event
        {
            Id = 3,
            Title = "OSU Symphony Orchestra: Winter Masterworks",
            Date = "2026-01-31",
            Location = "Mershon Auditorium, Columbus, OH",
            Description = "The Ohio State University Symphony Orchestra performs a program of classical masterworks.",
            AvailableTickets = 160,
            Price = 30
        },
        new Event
        {
            Id = 4,
            Title = "Columbus Crew vs. FC Cincinnati",
            Date = "2026-05-09",
            Location = "Lower.com Field, Columbus, OH",
            Description = "High-stakes MLS matchup in downtown Columbus with one of the league's best atmospheres.",
            AvailableTickets = 210,
            Price = 55
        },
        new Event
        {
            Id = 5,
            Title = "Summer Concert Night: Indie on the Scioto",
            Date = "2026-07-18",
            Location = "Scioto Mile, Columbus, OH",
            Description = "Outdoor concert featuring regional indie and alternative artists along the downtown riverfront.",
            AvailableTickets = 300,
            Price = 40
        },
        new Event
        {
            Id = 6,
            Title = "Columbus Food Truck Festival",
            Date = "2026-06-21",
            Location = "Columbus Commons, Columbus, OH",
            Description = "A citywide favorite with dozens of local food trucks, live music, and family-friendly activities.",
            AvailableTickets = 250,
            Price = 20
        }
    };

    // GET api/events


    // GET api/events/{id}

}
