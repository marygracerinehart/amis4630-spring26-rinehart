import { useState } from "react";

interface TicketCounterProps {
  eventTitle: string;
  maxTickets: number;
}

export function TicketCounter({ eventTitle, maxTickets }: TicketCounterProps) {
  const [quantity, setQuantity] = useState(0);

  const handleIncrement = () => {
    if (quantity < maxTickets) {
      setQuantity(quantity + 1);
    }
  };

  const handleDecrement = () => {
    if (quantity > 0) {
      setQuantity(quantity - 1);
    }
  };

  return (
    <div className="ticket-counter">
      <div className="counter-controls">
        <button
          onClick={handleDecrement}
          disabled={quantity === 0}
          aria-label="Decrease quantity"
        >
          −
        </button>
        <span className="count">{quantity}</span>
        <button
          onClick={handleIncrement}
          disabled={quantity >= maxTickets}
          aria-label="Increase quantity"
        >
          +
        </button>
      </div>
      {quantity > 0 && (
        <div className="selection-summary">
          {quantity} ticket{quantity !== 1 ? "s" : ""} selected for {eventTitle}.
        </div>
      )}
    </div>
  );
}
