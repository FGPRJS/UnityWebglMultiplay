using GameServer.Hub.Game;
using GameServer.Hub.Lobby;
using GameServer.Service;
using GameServer.Singletons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR(options =>
{
    options.KeepAliveInterval = TimeSpan.FromSeconds(10);
});
builder.Services.AddSingleton<GameManager>();
builder.Services.AddSingleton<LobbyManager>();

builder.Services.AddHostedService<HostedInitializer>();

var app = builder.Build();

app.MapHub<LobbyHub>("/lobby");
app.MapHub<GameHub>("/game");

// Configure the HTTP request pipeline.

app.Run();