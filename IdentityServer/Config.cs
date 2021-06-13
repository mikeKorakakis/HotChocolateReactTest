// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources()
        {
            return new IdentityResource[]
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            };
        }

        public static IEnumerable<ApiResource> Apis()
        {
           return new ApiResource[] {
                new ApiResource ("api1", "My API"){ Scopes = { "read", "write", "delete"}}
                   };
        }
        public static IEnumerable<ApiScope> ApiScopes()
        {
            return new List<ApiScope>
                {
                    new ApiScope(name: "read",   displayName: "Read your data."),
                    new ApiScope(name: "write",  displayName: "Write your data."),
                    new ApiScope(name: "delete", displayName: "Delete your data.")
                };
        }

        public static IEnumerable<Client> Clients()
        {
            var client = Startup.StaticConfig["SpaClientUri"];
            return new Client[] {
                // client credentials flow client
                // new Client
                // {
                //     ClientId = "client",
                //     ClientName = "Client React App",

                //     AllowedGrantTypes = GrantTypes.ClientCredentials,
                //     ClientSecrets = { new Secret("secret".Sha256()) },

                //     AllowedScopes = { "api1" }
                // },
                new Client {
                ClientId = "spa",
                ClientName = "SPA Client",
                ClientUri = client,
                ClientSecrets = { new Secret ("secret".Sha256 ()) },
                AllowOfflineAccess = true,
                RequirePkce = false,
                AllowedGrantTypes = {GrantType.AuthorizationCode },
                RequireClientSecret = false,
                RequireConsent = false,

                PostLogoutRedirectUris =  {$"{client}/signout-oidc"},
                AllowedCorsOrigins = {client},
                RedirectUris = {$"{client}/signin-oidc"},
                UpdateAccessTokenClaimsOnRefresh = true,
                AllowedScopes = { "openid", "profile", "api1", "read", "write", "delete"  }

        }
        };


        }
    }
}