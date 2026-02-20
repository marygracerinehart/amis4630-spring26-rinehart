import { Event } from "../data/events";

interface EventListProps {
  events: Event[];
}

export function EventList({ events }: EventListProps) {
  if (!events || events.length === 0) {
    return <p>No events available.</p>;
  }

  return (
    <div className="event-list">
      {events.map((event) => (
        <h2>{event.title}</h2>
      ))}
    </div>
  );
}
