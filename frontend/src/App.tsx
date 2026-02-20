import { events } from "./data/events";
import { EventList } from "./components/EventList";
import "./App.css";

function App() {
  return (
    <div className="app">
      <h1>OSU Event Finder</h1>
      <EventList events={events} />
    </div>
  );
}

export default App;
