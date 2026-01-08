using Duende.IdentityServer.Models;

namespace ShopOnline.IdentityServer.Configuration
{
    public static class Config
    {
        public static IEnumerable<IdentityResource> IdentityResources =>
            [
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResource("roles", "Your role(s)", ["role"])
            ];

        public static IEnumerable<ApiScope> ApiScopes =>
            [
                new ApiScope("shop_online_api", "Shop Online Api")
            ];

        public static IEnumerable<Client> Clients =>
            [
                new Client
                {
                    ClientId = "shop_online_mvc_client",
                    ClientSecrets = { new Secret("this_is_a_long_secret".Sha256()) },
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUris = { "https://localhost:7068/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:7068/signout-callback-oidc" },
                    AllowedScopes = { "openid", "profile", "shop_online_api", "roles" },
                    RequirePkce = true,
                    AllowPlainTextPkce = false,                    
                    RequireConsent = false
                },
                new Client
                {
                    ClientId = "shop_online_react_client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false, // SPA does not use secrets.
                    RedirectUris = { "http://localhost:5173/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5173/signout-callback-oidc" },
                    AllowedCorsOrigins = { "http://localhost:5173" },
                    AllowedScopes = { "openid", "profile", "shop_online_api", "roles", "offline_access" },
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowPlainTextPkce = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                },
                new Client
                {
                    ClientId = "shop_online_angular_client",
                    AllowedGrantTypes = GrantTypes.Code,
                    RequireClientSecret = false, // SPA does not use secrets.
                    RedirectUris = { "http://localhost:4200/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:4200/signout-callback-oidc" },
                    AllowedCorsOrigins = { "http://localhost:4200" },
                    AllowedScopes = { "openid", "profile", "shop_online_api", "roles", "offline_access" },
                    RequirePkce = true,
                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                    AllowPlainTextPkce = false,
                    AllowOfflineAccess = true,
                    AlwaysIncludeUserClaimsInIdToken = true
                }
            ];

        public static IEnumerable<ApiResource> ApiResources =>
            [
                new ApiResource("shop_online_api", "Shop Online API")
                {
                    Scopes = { "shop_online_api" },
                    UserClaims = { "email", "name", "sub", "role" }
                }
            ];

    }
}
