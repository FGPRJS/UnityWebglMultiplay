using GameServer.Singletons;

namespace GameServer.Hub.Game
{
    public static class GameHubMethod
    {
        public static string PlayerConnected = "PlayerConnected";
    }

    public class GameHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly GameManager _gameManager;

        public GameHub(GameManager gameManager)
        {
            this._gameManager = gameManager;
        }
    }
}
