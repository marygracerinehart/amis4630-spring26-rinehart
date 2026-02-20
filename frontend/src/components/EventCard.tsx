import type { EventCardProps } from "../types/event";
import { TicketCounter } from "./TicketCounter";

export function EventCard({ event }: EventCardProps) {
  return (
    <div className="event-card">
      <div className="event-header">
        <h3>{event.title}</h3>
        <span className="category-badge">{event.category}</span>
      </div>
      <div className="event-details">
        <p className="date">
          <strong>Date:</strong> {event.date}
        </p>
        <p className="location">
          <strong>Venue:</strong> {event.location}
        </p>
      </div>
      <p className="description">{event.description}</p>
      <div className="event-footer">
        <span className="price">
          {event.price > 0 ? `$${event.price.toFixed(2)}` : "Free"} per ticket
        </span>
        <span className="tickets">{event.availableTickets} tickets available</span>
      </div>
      <TicketCounter eventTitle={event.title} maxTickets={event.availableTickets} />
    </div>
  );
}
