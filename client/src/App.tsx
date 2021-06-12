import { Switch, Route } from "react-router-dom";

import './App.css';
import Home from "./Home";

function App() {
  return (
    <main>
        <Switch>
            <Route path="/" component={Home}></Route>
        </Switch>
    </main>
  );
}

export default App;
