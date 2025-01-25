using auth1;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuthService>();


var app = builder.Build();



app.MapGet("/username", (HttpContext ctx, IDataProtectionProvider idp) =>
{
    var protector = idp.CreateProtector("auth-cookie");
    // var authCookie = ctx.Request.Cookies["auth"];
    var authCookie = ctx.Request.Cookies.FirstOrDefault(c => c.Key == "auth");
    var auth = authCookie.Value;
    var unprotectedUserValue = protector.Unprotect(auth);
    var user = unprotectedUserValue.Split(":")[1];

    return user;
});

app.MapGet("/login", (AuthService authService) =>
{
    authService.SignIn();
    return "ok";
});




app.Run();
