using ReverseProxy.Api.ReverseProxyConfiguration;
using Scalar.AspNetCore;
using Yarp.ReverseProxy.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IConfigurator, Configurator>();

builder.Services.AddReverseProxy()
    .LoadFromMemory(DefaultConfiguration.GetRoutes(), DefaultConfiguration.GetClusters());

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.MapReverseProxy();

app.UseHttpsRedirection();

app.MapPost("/change-address/{address}", (string address) =>
    {
        var configurator = app.Services.GetRequiredService<IConfigurator>();
        var newAddress = $"https://{address}";

        configurator.ChangeDestination(newAddress);

        return Results.Ok($"Address was changed! Address{newAddress}");
    })
.WithName("ChangeDestinationAddress");

await app.RunAsync();