using System.Text.Json;
using GameServer.Singletons;

namespace GameServer.Hub.Game
{
    public static class GameMethod
    {
        public static string EnterGame = "EnterGame";
        public static string PlayerConnected = "PlayerConnected";
    }

    public class GameHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly GameManager _gameManager;

        public GameHub(GameManager gameManager)
        {
            this._gameManager = gameManager;
        }

        public async Task EnterGame(string gameId, string playerName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);

            var currentGamePlayInfo = this._gameManager.EnterGame(gameId, playerName);
            var currentGamePlayInfoJson = JsonSerializer.Serialize(currentGamePlayInfo);

            await Clients.Group(gameId).SendCoreAsync(GameMethod.PlayerConnected, new object[] { currentGamePlayInfoJson });
            await Clients.Caller.SendCoreAsync(GameMethod.EnterGame, new object[]{currentGamePlayInfoJson});
        }

        public async Task PlayerConnected(string gameId, string playerName)
        {
            await Clients.Group(gameId).SendCoreAsync(GameMethod.PlayerConnected, new object[] { gameId, playerName });
        }
    }
}
