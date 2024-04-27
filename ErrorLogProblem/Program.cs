using ErrorLogProblem;
using MassTransit;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Testcontainers.ActiveMq;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var activemq = new ArtemisBuilder()
    .WithPassword("admin")
    .WithUsername("admin")
    .WithPortBinding(8161, 8161)
    .Build();
await activemq.StartAsync();

builder.Services.Configure<MasstransitOptions>(builder.Configuration
    .GetSection(nameof(MasstransitOptions)));
builder.Services.TryAddSingleton(KebabCaseEndpointNameFormatter.Instance);
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MessageConsumer>();
    x.UsingActiveMq((context, cfg) =>
    {
        var options = context.GetRequiredService<IOptions<MasstransitOptions>>();
        var port = activemq.GetMappedPublicPort(61616);
        cfg.EnableArtemisCompatibility();
        cfg.Host(
            activemq.Hostname,
            port,
            h =>
            {
                h.Password("admin");
                h.Username("admin");
            });
        cfg.AutoStart = options.Value.Autostart;
        cfg.ConfigureEndpoints(context,
            new KebabCaseEndpointNameFormatter("Test", false));
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (IPublishEndpoint publishEndpoint) =>
    {
        await publishEndpoint.Publish(new Message { Greeting = "Hello world!" }); 
        var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        )).ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
