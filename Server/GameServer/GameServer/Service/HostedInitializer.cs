using GameServer.Singletons;

namespace GameServer.Service
{
    public class HostedInitializer : IHostedService
    {
        private readonly GameManager _gameManager;
        private readonly LobbyManager _lobbyManager;

        public HostedInitializer(GameManager gameManager, LobbyManager lobbyManager)
        {
            this._gameManager = gameManager;
            this._lobbyManager = lobbyManager;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
