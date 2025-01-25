using System.Security.Claims;
using auth1;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();


var app = builder.Build();

// Custom middleware
app.Use((ctx, next) =>
{
    var idp = ctx.RequestServices.GetRequiredService<IDataProtectionProvider>();
    var protector = idp.CreateProtector("auth-cookie");
    // var authCookie = ctx.Request.Cookies["auth"];
    var authCookie = ctx.Request.Cookies.FirstOrDefault(c => c.Key == "auth");
    var auth = authCookie.Value;
    var unprotectedUserValue = protector.Unprotect(auth);
    var key = unprotectedUserValue.Split(":")[0];
    var value = unprotectedUserValue.Split(":")[1];

    var claims = new List<Claim>();
    claims.Add(new Claim(key, value));
    var identity = new ClaimsIdentity(claims);
    var principal = new ClaimsPrincipal(identity);

    ctx.User = principal;

    return next();
});



app.MapGet("/username", (HttpContext ctx) =>
{

    return ctx.User.FindFirst("usr")?.Value;
});

app.MapGet("/login", (AuthService authService) =>
{
    authService.SignIn();
    return "ok";
});




app.Run();
