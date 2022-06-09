var builder = WebApplication.CreateBuilder();
var app = builder.Build();

app.Environment.EnvironmentName = "Test";   // измен€ем название среды на Test

if (app.Environment.IsEnvironment("Test")) // ≈сли проект в состо€нии "Test"
{
    app.Run(async (context) => await context.Response.WriteAsync("In Test Stage"));
}
else
{
    app.Run(async (context) => await context.Response.WriteAsync("In Development or Production Stage"));
}

app.Run();