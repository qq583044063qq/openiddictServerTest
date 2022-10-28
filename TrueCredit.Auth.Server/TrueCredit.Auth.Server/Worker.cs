using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.EntityFrameworkCore.Models;
using TrueCredit.Auth.Server.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace TrueCredit.Auth.Server
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public Worker(IServiceProvider serviceProvider)
            => _serviceProvider = serviceProvider;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await context.Database.EnsureCreatedAsync();

            await CreateApplicationsAsync();
            await CreateScopesAsync();

            async Task CreateApplicationsAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();
                // 面向公共登录站点的授权设置
                if (await manager.FindByClientIdAsync("aurelia") == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = "aurelia",
                        DisplayName = "Aurelia client application",
                        PostLogoutRedirectUris =
                        {
                            new Uri("http://127.0.0.1:5500/index.html")
                        },
                        RedirectUris =
                        {
                            new Uri("http://127.0.0.1:5500/callback.html")
                        },
                        Permissions =
                        {
                            Permissions.Endpoints.Authorization,
                            Permissions.Endpoints.Logout,
                            Permissions.Endpoints.Token,
                            Permissions.GrantTypes.Implicit,
                            Permissions.GrantTypes.AuthorizationCode,
                            Permissions.GrantTypes.Password,
                            Permissions.ResponseTypes.IdToken,
                            Permissions.ResponseTypes.IdTokenToken,
                            Permissions.ResponseTypes.Token,
                            Permissions.ResponseTypes.Code,
                            Permissions.Scopes.Email,
                            Permissions.Scopes.Profile,
                            Permissions.Scopes.Roles,
                            Permissions.Prefixes.Scope + "api1",
                            Permissions.Prefixes.Scope + "api2"
                        }
                    };

                    await manager.CreateAsync(descriptor);

                }
                else
                {
                    // 修改一个存在的授权配置
                    //var app = manager.FindByClientIdAsync("aurelia").Result;
                    //var descriptor = new OpenIddictApplicationDescriptor
                    //{
                    //    ClientId = "aurelia",
                    //    Type = ClientTypes.Public,
                    //    DisplayName = "Aurelia client application",
                    //    PostLogoutRedirectUris =
                    //    {
                    //        new Uri("http://127.0.0.1:5500/index.html")
                    //    },
                    //    RedirectUris =
                    //    {
                    //        new Uri("http://127.0.0.1:5500/callback.html")
                    //    },
                    //    Permissions =
                    //    {
                    //        Permissions.Endpoints.Authorization,
                    //        Permissions.Endpoints.Logout,
                    //        Permissions.Endpoints.Token,
                    //        Permissions.GrantTypes.Implicit,
                    //        Permissions.GrantTypes.AuthorizationCode,
                    //        Permissions.GrantTypes.Password,
                    //        Permissions.ResponseTypes.IdToken,
                    //        Permissions.ResponseTypes.IdTokenToken,
                    //        Permissions.ResponseTypes.Token,
                    //        Permissions.ResponseTypes.Code,
                    //        Permissions.Scopes.Email,
                    //        Permissions.Scopes.Profile,
                    //        Permissions.Scopes.Roles,
                    //        Permissions.Prefixes.Scope + "api1",
                    //        Permissions.Prefixes.Scope + "api2"
                    //    }
                    //};
                    //await manager.UpdateAsync(app, descriptor);
                    //var descriptor = new OpenIddictApplicationDescriptor
                    //{
                    //    ClientId = "aurelia",
                    //    DisplayName = "Aurelia client application",
                    //    PostLogoutRedirectUris =
                    //    {
                    //        new Uri("https://localhost:44398/signout-oidc")
                    //    },
                    //    RedirectUris =
                    //    {
                    //        new Uri("https://localhost:44342/api")
                    //    },
                    //    Permissions =
                    //    {
                    //        Permissions.Endpoints.Authorization,
                    //        Permissions.Endpoints.Logout,
                    //        Permissions.GrantTypes.Implicit,
                    //        Permissions.ResponseTypes.IdToken,
                    //        Permissions.ResponseTypes.IdTokenToken,
                    //        Permissions.ResponseTypes.Token,
                    //        Permissions.Scopes.Email,
                    //        Permissions.Scopes.Profile,
                    //        Permissions.Scopes.Roles,
                    //        Permissions.Prefixes.Scope + "api1",
                    //        Permissions.Prefixes.Scope + "api2"
                    //    }
                    //};
                    //await manager.UpdateAsync(descriptor);
                }

                // 内部api资源的授权设置
                if (await manager.FindByClientIdAsync("resource_server_1") == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = "resource_server_1",
                        ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342",
                        Permissions =
                        {
                            Permissions.Endpoints.Introspection
                        }
                    };

                    await manager.CreateAsync(descriptor);
                }
                // Note: no client registration is created for resource_server_2
                // as it uses local token validation instead of introspection.
            }

            async Task CreateScopesAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();

                if (await manager.FindByNameAsync("api1") == null)
                {
                    var descriptor = new OpenIddictScopeDescriptor
                    {
                        Name = "api1",
                        Resources =
                        {
                            "resource_server_1"
                        }
                    };

                    await manager.CreateAsync(descriptor);
                }

                if (await manager.FindByNameAsync("api2") == null)
                {
                    var descriptor = new OpenIddictScopeDescriptor
                    {
                        Name = "api2",
                        Resources =
                        {
                            "resource_server_2"
                        }
                    };

                    await manager.CreateAsync(descriptor);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
