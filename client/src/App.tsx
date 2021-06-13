import { Switch, Route } from "react-router-dom";

import './App.css';
import Home from "./Components/Home";
import SigninOidc from './Components/oidc/SigninOIDC';
import SignoutOidc from './Components/oidc/SignoutOIDC';

function App() {
  return (
    <main>
        <Switch>
            <Route path="/" component={Home} exact></Route>
            <Route path="/signin-oidc" component={SigninOidc}></Route>
            <Route path="/signout-oidc" component={SignoutOidc}></Route>
        </Switch>
    </main>
  );
}

export default App;
