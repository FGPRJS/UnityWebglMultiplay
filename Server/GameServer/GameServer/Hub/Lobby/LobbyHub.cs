using GameServer.Model;
using GameServer.Singletons;

namespace GameServer.Hub.Lobby
{
    public static class LobbyMethod
    {
        public const string IssueTicket = "IssueTicket";
        public const string CancelTicket = "CancelTicket";
        public const string TicketMatched = "TicketMatched";
    }

    public class LobbyHub : Microsoft.AspNetCore.SignalR.Hub
    {
        private readonly GameManager _gameManager;
        private readonly LobbyManager _lobbyManager;

        public LobbyHub(GameManager gameManager, LobbyManager lobbyManager)
        {
            this._gameManager = gameManager;
            this._lobbyManager = lobbyManager;
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            return base.OnDisconnectedAsync(exception);
        }

        

        public async Task IssueTicket(string playerName)
        {
            var caller = this.Clients.Caller;

            var ticketToken = this._lobbyManager.IssueTicket(new MatchingPlayer()
            {
                client = caller,
                playerName = playerName,
                status = PlayerStatus.Matching
            });

            await caller.SendCoreAsync(LobbyMethod.IssueTicket, new object[] { ticketToken });
        }

        public async Task CancelTicket(string ticketToken)
        {
            this._lobbyManager.CancelTicket(ticketToken);

            await this.Clients.Caller.SendCoreAsync(
                LobbyMethod.CancelTicket, 
                new object[] { ticketToken });
        }
    }
}
