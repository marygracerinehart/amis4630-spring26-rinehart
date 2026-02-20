import { EventCard } from "./EventCard"; 
import type { EventListProps } from "../types/event";

export function EventList({ events, onSelectEvent }: EventListProps) {
  if (!events || events.length === 0) {
    return <p>No events available.</p>;
  }

  return (
    <div className="event-list">
      {events.map((event) => (
        <div
          key={event.id}
          onClick={() => onSelectEvent?.(event)}
          style={{ cursor: onSelectEvent ? "pointer" : "default" }}
        >
          <EventCard event={event} />
        </div>
      ))}
    </div>
  );
}
