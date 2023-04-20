using GameServer.Hub.Game;
using GameServer.Singletons;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSignalR();
builder.Services.AddSingleton<GameManager>();

var app = builder.Build();

app.MapHub<GameHub>("/game");

// Configure the HTTP request pipeline.

app.Run();