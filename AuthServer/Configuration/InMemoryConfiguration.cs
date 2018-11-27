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
                }
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

    }
}
