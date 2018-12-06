using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;

namespace AuthServer.Configuration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> ApiResources()
        {
            return new[]
            {
                new ApiResource("socialnetwork", "社交网络")
            };
        }

        public static IEnumerable<Client> Clients()
        {
            return new[] {
                new Client{
                    ClientId ="socialnetwork",
                    ClientSecrets=new[] { new Secret("Secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes = new[]{ "socialnetwork" }
                },
                //ClientId要和MvcClient里面指定的名称一致.
                new Client
                {
                    ClientId = "mvc_implicit",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    //RedirectUris就是登陆成功之后重定向的网址
                    RedirectUris = { "https://localhost:7001/signin-oidc" },
                    PostLogoutRedirectUris = { "https://localhost:7001/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork"
                    },
                    //如果不加这个，可以看到id_token, 而access_token没有,
                    //这是因为我们还没有告诉Authorization Server在使用implicit flow时可以允许返回Access token.
                    //我们需要重新登陆来获取access token
                    AllowAccessTokensViaBrowser = true
                },
                // JavaScript Client
                new Client
                {
                    ClientId = "js_client",
                    ClientName = "JavaScript Client",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,

                    RedirectUris =           { "http://localhost:5003/callback.html" },
                    PostLogoutRedirectUris = { "http://localhost:5003/index.html" },
                    AllowedCorsOrigins =     { "http://localhost:5003" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "socialnetwork"
                    }
                
                },
            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "632494788@qq.com",
                    Password = "password"
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }
    }
}
