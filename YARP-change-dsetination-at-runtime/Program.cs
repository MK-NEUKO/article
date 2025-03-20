var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapReverseProxy();

app.UseHttpsRedirection();

app.MapGet("/weatherforecast", () =>
{
    
})
.WithName("GetWeatherForecast");

await app.RunAsync();