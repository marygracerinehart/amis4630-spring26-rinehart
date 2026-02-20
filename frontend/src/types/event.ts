/** Supported event categories */
export type EventCategory = "Sports" | "Music" | "Arts & Culture" | "Food & Beverage" | "Other";

/**
 * Represents an event with ticketing information.
 * Aligns with the backend Event model and provides
 * type safety throughout the frontend application.
 */
export interface Event {
  /** Unique identifier for the event */
  readonly id: number;

  /** Event title or name */
  readonly title: string;

  /** Event date in ISO 8601 format (YYYY-MM-DD) */
  readonly date: string;

  /** Physical location or venue name */
  readonly location: string;

  /** Detailed description of the event */
  readonly description: string;

  /** Event category for filtering and organization */
  readonly category: EventCategory;

  /** Number of tickets available for purchase */
  readonly availableTickets: number;

  /** Price per ticket in USD */
  readonly price: number;
}

/**
 * Props for components that display a single event.
 */
export interface EventCardProps {
  event: Event;
  onPurchase?: (event: Event, quantity: number) => void;
}

/**
 * Props for components that display a list of events.
 */
export interface EventListProps {
  events: Event[];
  onSelectEvent?: (event: Event) => void;
}
