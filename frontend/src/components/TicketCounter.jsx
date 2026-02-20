import { useState } from 'react'

export function TicketCounter({ availableTickets }) {
  const [count, setCount] = useState(0)

  const decrement = () => setCount((c) => Math.max(0, c - 1))
  const increment = () => setCount((c) => Math.min(availableTickets, c + 1))

  return (
    <div className="ticket-counter">
      <p>Available tickets: {availableTickets}</p>
      <div className="counter-controls">
        <button onClick={decrement} disabled={count === 0}>−</button>
        <span className="count">{count}</span>
        <button onClick={increment} disabled={count === availableTickets}>+</button>
      </div>
      {count > 0 && (
        <p className="selection-summary">Selected: {count} ticket{count !== 1 ? 's' : ''}</p>
      )}
    </div>
  )
}
