import React from "react";
import ReactDOM from "react-dom";
import "./index.css";
import App from "./App";
import { BrowserRouter } from "react-router-dom";
import { ApolloClient, InMemoryCache, ApolloProvider } from "@apollo/client";
import { setContext } from '@apollo/client/link/context';
import {  createHttpLink, ApolloLink} from '@apollo/client';
import userManager from "./Components/oidc/userService";
import { WebSocketLink } from "@apollo/client/link/ws";
import { getOperationAST } from "graphql";


const graphqlUri = "http://localhost:5000/graphql"
const wsGraphqlUri = "ws://localhost:5000/graphql"

let link: any;
const httpLink = createHttpLink({
  uri: graphqlUri
});


const authLink = setContext(async(_, { headers }) => {
  // get the authentication token from local storage if it exists
//   const token = localStorage.getItem('token');
  const user =  await userManager.getUser()
  const token = user?.access_token;
  // return the headers to the context so httpLink can read them
  return {
    headers: {
      ...headers,
      authorization: token ? `Bearer ${token}` : "",
    }
  }
});

let wsClient = new WebSocketLink({
    uri: wsGraphqlUri,
    options: {
        reconnect: true,
        lazy: true,
        connectionParams: async () => {
            const user =  await userManager.getUser()
            const token = user?.access_token;
            return {authToken:  token}
        },
    },
});

link = ApolloLink.split(
    (operation) => {
        const operationAST = getOperationAST(
            operation.query,
            operation.operationName
        );
        return (
            !!operationAST && operationAST.operation === "subscription"
        );
    },
    wsClient,
    httpLink
);

const client = new ApolloClient({
    link: ApolloLink.from([authLink, link]),
    cache: new InMemoryCache()
  });
  
ReactDOM.render(
	<React.StrictMode>
		<ApolloProvider client={client}>
			<BrowserRouter>
				<App />
			</BrowserRouter>
		</ApolloProvider>
	</React.StrictMode>,
	document.getElementById("root")
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
