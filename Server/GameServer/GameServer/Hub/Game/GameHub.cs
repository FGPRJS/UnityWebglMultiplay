namespace GameServer.Hub.Game
{
    public class GameHub : Microsoft.AspNetCore.SignalR.Hub
    {
        public async Task PlayerConnected(string playerName)
        {
            await Clients.Others.SendCoreAsync(
                GameHubMethod.PlayerConnected,
                new object[]{ playerName });
        }
    }
}
