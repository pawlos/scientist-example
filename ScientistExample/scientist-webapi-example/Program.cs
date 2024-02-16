using System.Security.Cryptography;
using GitHub;
using scientist_webapi_example;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/weatherforecast-old", () =>
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecastOld");
app.MapGet("/weatherforecast", () =>
    {
        return Scientist.Science<WeatherForecast[]>("weather-forecast", experiment =>
        {
            // https://github.com/scientistproject/Scientist.net
            experiment.Use(WeatherForecastsOld); // old
            experiment.Try("RandomNumberGenerator", WeatherForecastsNew); // experiment 1
            experiment.Try("Static", WeatherForecastsStatic); // experiment 2
            /*
             * Define custom comparison logic
             * experiment.Compare((x, y) => x.Name == y.Name);
             *
             * Add extra context
             * experiment.AddContext("username", userName);
             *
             * Prepare data before running normal run/experiment
             * experiment.BeforeRun(() => ExpensiveSetup());
             *
             * Decide what to store (drop password)
             * experiment.Clean(user => user.Login);
             *
             * Decide when match/mismatch is made
             * experiment.Ignore((control, candidate) => user.IsStaff);
             *
             * Decide when to run an experiment
             * experiment.RunIf(() => user.IsTestSubject);
             */
        });
    })
    .WithName("GetWeatherForecast");

// we can wrap FireAndForgetResultPublisher
Scientist.ResultPublisher = new ResultsPublisher();
app.Run();

WeatherForecast[] WeatherForecastsOld()
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                summaries[Random.Shared.Next(summaries.Length)]
            ))
        .ToArray();
    return forecast;
}

WeatherForecast[] WeatherForecastsNew()
{
    var summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };
    var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                RandomNumberGenerator.GetInt32(-20, 55),
                summaries[RandomNumberGenerator.GetInt32(summaries.Length)]
            ))
        .ToArray();
    return forecast;
}

WeatherForecast[] WeatherForecastsStatic()
{
    return Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast(
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            100,
            "Scorching")).ToArray();
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    public override string ToString()
    {
        return "Temp: " + TemperatureC + "C, " + Summary + " on " + Date.ToString("d") + " (" + TemperatureF + "F)";
    }
}