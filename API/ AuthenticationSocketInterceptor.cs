using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using HotChocolate;
using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Messages;
using HotChocolate.Execution;
// using HotChocolate.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace API
{
    public class AuthenticationSocketInterceptor : DefaultSocketSessionInterceptor
    {
        // This is the key to the auth token in the HTTP Context
        public static readonly string HTTP_CONTEXT_WEBSOCKET_AUTH_KEY = "websocket-auth-token";
        // This is the key that apollo uses in the connection init request
        public static readonly string WEBOCKET_PAYLOAD_AUTH_KEY = "authToken";

        // private readonly IAuthenticationSchemeProvider _schemes;
        // public AuthenticationSocketInterceptor( IAuthenticationSchemeProvider schemes)
        // {
        //     _schemes = schemes;
        // }



        public override async ValueTask<ConnectionStatus> OnConnectAsync(ISocketConnection connection, InitializeConnectionMessage message, CancellationToken cancellationToken)
        {


            HttpContext context = connection.HttpContext;
            if (message.Payload!.TryGetValue(WEBOCKET_PAYLOAD_AUTH_KEY, out object token) &&
             token is string stringToken)
            {
                context.Items[HTTP_CONTEXT_WEBSOCKET_AUTH_KEY] = stringToken;
                context.Features.Set<IAuthenticationFeature>(new AuthenticationFeature
                {
                    OriginalPath = context.Request.Path,
                    OriginalPathBase = context.Request.PathBase
                });
                // Give any IAuthenticationRequestHandler schemes a chance to handle the request
                var handlers = context.RequestServices.GetRequiredService<IAuthenticationHandlerProvider>();
                var _schemes = context.RequestServices.GetRequiredService<IAuthenticationSchemeProvider>();
                foreach (var scheme in await _schemes.GetRequestHandlerSchemesAsync())
                {
                    var handler = handlers.GetHandlerAsync(context, scheme.Name) as IAuthenticationRequestHandler;
                    if (handler != null && await handler.HandleRequestAsync())
                    {
                        return (ConnectionStatus.Reject());

                    }
                }
                var defaultAuthenticate = await _schemes.GetDefaultAuthenticateSchemeAsync();
                if (defaultAuthenticate != null)
                {
                //  return (ConnectionStatus.Accept());
                    var result = await context.AuthenticateAsync(defaultAuthenticate.Name);
                    if (result.Principal != null)
                    {
                        var webSocketContext = context.RequestServices.GetService<WebSocketContext>();        
                        context.User = result.Principal;
                        return (ConnectionStatus.Accept());
                    }
                }
            }
            return (ConnectionStatus.Reject());

        }

        public override ValueTask OnRequestAsync(ISocketConnection connection, IQueryRequestBuilder requestBuilder, CancellationToken cancellationToken)
        {
            return base.OnRequestAsync(connection, requestBuilder, cancellationToken);
        }

        public override ValueTask OnCloseAsync(ISocketConnection connection, CancellationToken cancellationToken)
        {
            return base.OnCloseAsync(connection, cancellationToken);
        }



    }
}