export interface Event {
    id: number;
    title: string;
    date: string;
    location: string;
    description: string;
    availableTickets: number;
    price: number;
}

export const events: Event[] = [
    {
        id: 1,
        title: "Ohio State Buckeyes Football vs. Penn State",
        date: "2026-10-24",
        location: "Ohio Stadium, Columbus, OH",
        description:
            "Big Ten showdown at The Horseshoe featuring Ohio State football under the lights.",
        availableTickets: 120,
        price: 145,
    },
    {
        id: 2,
        title: "Ohio State Buckeyes Men's Basketball vs. Michigan",
        date: "2026-02-14",
        location: "Value City Arena, Columbus, OH",
        description:
            "Rivalry game in Columbus with conference implications and high-energy crowd support.",
        availableTickets: 95,
        price: 85,
    },
    {
        id: 3,
        title: "OSU Symphony Orchestra: Winter Masterworks",
        date: "2026-01-31",
        location: "Mershon Auditorium, Columbus, OH",
        description:
            "The Ohio State University Symphony Orchestra performs a program of classical masterworks.",
        availableTickets: 160,
        price: 30,
    },
    {
        id: 4,
        title: "Columbus Crew vs. FC Cincinnati",
        date: "2026-05-09",
        location: "Lower.com Field, Columbus, OH",
        description:
            "High-stakes MLS matchup in downtown Columbus with one of the league's best atmospheres.",
        availableTickets: 210,
        price: 55,
    },
    {
        id: 5,
        title: "Summer Concert Night: Indie on the Scioto",
        date: "2026-07-18",
        location: "Scioto Mile, Columbus, OH",
        description:
            "Outdoor concert featuring regional indie and alternative artists along the downtown riverfront.",
        availableTickets: 300,
        price: 40,
    },
    {
        id: 6,
        title: "Columbus Food Truck Festival",
        date: "2026-06-21",
        location: "Columbus Commons, Columbus, OH",
        description:
            "A citywide favorite with dozens of local food trucks, live music, and family-friendly activities.",
        availableTickets: 250,
        price: 20,
    },
];
