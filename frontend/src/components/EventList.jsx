import { EventCard } from './EventCard'

export function EventList({ events }) {
  if (!events || events.length === 0) {
    return <p>No events available.</p>
  }

  return (
    <div className="event-list">
      {events.map((event) => (
        <EventCard key={event.id} event={event} />
      ))}
    </div>
  )
}
