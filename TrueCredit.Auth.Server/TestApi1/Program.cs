using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;
using TestApi1;

var builder = WebApplication.CreateBuilder(args);

//¿çÓòÅäÖÃ
builder.Services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
{
    // ÉèÖÃÎªÔÊÐíËùÓÐ¿çÓò
    builder.SetIsOriginAllowed(_ => true).AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}));
// Register the OpenIddict validation components.
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // Note: the validation handler uses OpenID Connect discovery
        // to retrieve the address of the introspection endpoint.
        options.SetIssuer("https://localhost:44330/");
        options.AddAudiences("resource_server_1");

        // Configure the validation handler to use introspection and register the client
        // credentials used when communicating with the remote introspection endpoint.
        options.UseIntrospection()
               .SetClientId("resource_server_1")
               .SetClientSecret("846B62D0-DEF9-4215-A99D-86E6B8DAB342");

        // Register the System.Net.Http integration.
        options.UseSystemNetHttp();

        // Register the ASP.NET Core host.
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
// ÅäÖÃÊÚÈ¨²ßÂÔ
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(UserPolicyName.User, policy => policy.RequireRole(UserRoleName.User, UserRoleName.Administrator, UserRoleName.MasterAdministrator));
    options.AddPolicy(UserPolicyName.Administrator, policy => policy.RequireRole(UserRoleName.Administrator, UserRoleName.MasterAdministrator));
    options.AddPolicy(UserPolicyName.MasterAdministrator, policy => policy.RequireRole(UserRoleName.MasterAdministrator));
});

var app = builder.Build();
//Ê¹ÓÃ¿çÓòÅäÖÃ
app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api", [Authorize][Authorize(Policy = UserPolicyName.User)] (ClaimsPrincipal user) => $"{user.Identity!.Name} is allowed to access Api1.");

app.Run();
