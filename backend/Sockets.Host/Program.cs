using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sockets.Hubs;

// Create a host that can cohost aspnetcore AND orleans together in a single process.
var builder = WebApplication.CreateBuilder(args);
builder.Host.UseOrleans(siloBuilder =>
{
    siloBuilder.UseLocalhostClustering()
    .ConfigureLogging(logging => logging.AddConsole());
    siloBuilder.UseSignalR(); // Adds ability #1 and #2 to Orleans.
    siloBuilder.RegisterHub<MyHub>(); // Required for each hub type if the backplane ability #1 is being used.
});


builder.Services
    .AddSignalR()  // Adds SignalR hubs to the web application
    .AddOrleans(); // Tells the SignalR hubs in the web application to use Orleans as a backplane (ability #1)

builder.Services.AddCors(options =>
{
	options.AddPolicy(name: "AllowAllCors",
        policy =>
	    {
            policy.AllowAnyHeader();
            policy.AllowCredentials();
            policy.WithOrigins("http://localhost:5173");
	    });
});

var app = builder.Build();
app.UseStaticFiles();
app.UseDefaultFiles();
app.UseCors("AllowAllCors");
app.MapHub<MyHub>("/myhub");

await app.RunAsync();
