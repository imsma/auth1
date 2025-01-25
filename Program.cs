using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

const string AuthSchme = "cookie";

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(AuthSchme)
        .AddCookie(AuthSchme);


var app = builder.Build();

app.UseAuthentication();

app.MapGet("/login", async (HttpContext ctx) =>
{

    var claims = new List<Claim>
    {
        new Claim("usr", "sma")
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






app.Run();
