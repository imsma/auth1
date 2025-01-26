using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

const string AuthSchme = "cookie";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(AuthSchme)
        .AddCookie(AuthSchme);

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("European", policy =>
    {
        policy.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(AuthSchme)
        .RequireClaim("passport-type", "eur");
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/login", async (HttpContext ctx) =>
{

    var claims = new List<Claim>
    {
        new Claim("usr", "sma"),
        new Claim("passport-type", "eur")
    };

    var identity = new ClaimsIdentity(claims, AuthSchme);
    var user = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(AuthSchme, user);

}).AllowAnonymous();


app.MapGet("/unsecure", (HttpContext ctx) =>
{
    var user = ctx.User;
    var username = user.FindFirst("usr")?.Value ?? "Anonymous";

    return username;
});

app.MapGet("/sweden", (HttpContext ctx) =>
{
    return "Allowed";
}).RequireAuthorization("European");


app.MapGet("/norway", (HttpContext ctx) =>
{
    // var isLoggedIn = ctx.User.Identities.Any(x => x.AuthenticationType == AuthSchme);
    // if (!isLoggedIn)
    // {
    //     ctx.Response.StatusCode = 401;
    //     return "";
    // }

    // var isEuropean = ctx.User.HasClaim("passport-type", "nor");
    // if (!isEuropean)
    // {
    //     ctx.Response.StatusCode = 403;
    //     return "";
    // }

    return "Allowed";
});

app.MapGet("/denmark", (HttpContext ctx) =>
{
    // var isLoggedIn = ctx.User.Identities.Any(x => x.AuthenticationType == AuthSchme);
    // if (!isLoggedIn)
    // {
    //     ctx.Response.StatusCode = 401;
    //     return "";
    // }

    // var isEuropean = ctx.User.HasClaim("passport-type", "eur");
    // if (!isEuropean)
    // {
    //     ctx.Response.StatusCode = 403;
    //     return "";
    // }

    return "Allowed";
});





app.Run();

public class MyRequirement : IAuthorizationRequirement
{
    public MyRequirement() { }

}

public class MyRequirementHandler : AuthorizationHandler<MyRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MyRequirement requirement)
    {
        if (context.User.HasClaim("passport-type", "eur"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
