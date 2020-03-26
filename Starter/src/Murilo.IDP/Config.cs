﻿using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace Murilo.IDP
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> Ids =>
            new IdentityResource[]
            { 
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Address(),
                new IdentityResource(
                    "roles",
                    "Your role(s)",
                    new List<string>() { "role" }),
                new IdentityResource(
                    "country",
                    "The country you're living in",
                    new List<string>() { "country" }
                ),
                new IdentityResource(
                    "subscriptionlevel",
                    "Your subscription level",
                    new List<string>() { "subscriptionlevel" }
                )
            };

        public static IEnumerable<ApiResource> Apis =>
            new ApiResource[] 
            { 
                new ApiResource("imagegalleryapi", 
                                "Image Gallery API",
                                new List<string>() { "role" }),
            };
        
        public static IEnumerable<Client> Clients =>
            new Client[] 
            { 
                new Client{
                    ClientName = "Image Gallery",
                    ClientId = "imagegalleryclient",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    
                    RedirectUris = new List<string>()
                    {
                        "http://localhost:44369/signin-oidc"
                    },
                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId, 
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Address,
                        "roles",
                        "imagegalleryapi",
                        "country",
                        "subscriptionlevel",
                    },
                    ClientSecrets = {
                        new Secret("secret".Sha256())
                    }
                }
            };
        
    }
}