using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string AuthSchme = "cookie";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(AuthSchme)
        .AddCookie(AuthSchme);


var app = builder.Build();

app.UseAuthentication();

app.Use((ctx, next) =>
{
    if (ctx.Request.Path.StartsWithSegments("/login"))
    {
        return next();
    }

    if (!ctx.User.Identities.Any(x => x.AuthenticationType == AuthSchme))
    {
        ctx.Response.StatusCode = 401;
        return Task.CompletedTask;
    }

    if (!ctx.User.HasClaim("passport-type", "eur"))
    {
        ctx.Response.StatusCode = 403;
        return Task.CompletedTask;
    }

    return next();
});

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

});


app.MapGet("/unsecure", (HttpContext ctx) =>
{
    var user = ctx.User;
    var username = user.FindFirst("usr")?.Value ?? "Anonymous";

    return username;
});

app.MapGet("/sweden", (HttpContext ctx) =>
{
    return "Allowed";
});


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
