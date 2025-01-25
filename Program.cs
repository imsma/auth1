var builder = WebApplication.CreateBuilder(args);


var app = builder.Build();

app.MapGet("/username", () =>
{

});

app.MapGet("/login", (HttpContext ctx) =>
{

});




app.Run();
