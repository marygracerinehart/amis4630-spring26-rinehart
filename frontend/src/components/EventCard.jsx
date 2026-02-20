import { TicketCounter } from './TicketCounter'

export function EventCard({ event }) {
  const { title, date, location, description, availableTickets, price } = event

  return (
    <div className="event-card">
      <h2>{title}</h2>
      <p className="event-date">📅 {date}</p>
      <p className="event-location">📍 {location}</p>
      <p className="event-description">{description}</p>
      <p className="event-price">Price: ${price}</p>
      <TicketCounter availableTickets={availableTickets} />
    </div>
  )
}
