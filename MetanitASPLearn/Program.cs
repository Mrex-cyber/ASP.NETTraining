var builder = WebApplication.CreateBuilder();
builder.Services.AddTransient<ITimeService, GetShortTimeService>();
var app = builder.Build();


app.Environment.EnvironmentName = "Test";   

app.Run(async context =>
{
    var timeService = app.Services.GetService<ITimeService>();
    await context.Response.WriteAsync($"Time: {timeService?.GetTime()}");
});

if (app.Environment.IsEnvironment("Test"))
{
    app.Run(async (context) => await context.Response.WriteAsync("In Test Stage"));
}
else
{
    app.Run(async (context) => await context.Response.WriteAsync("In Development or Production Stage"));
}

app.Run();

interface ITimeService
{
    string GetTime();
}

class GetShortTimeService : ITimeService
{
    public string GetTime() => DateTime.Now.ToShortTimeString();
}
class GetlongTimeService : ITimeService
{
    public string GetTime() => DateTime.Now.ToLongTimeString();
}