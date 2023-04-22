using Microsoft.AspNetCore.SignalR;

namespace GameServer.Model
{
    public enum PlayerStatus
    {
        Matching = 0,
        Canceled = 1
    }

    public enum QueueMatchStatus
    {
        Idle = 0,
        Busy = 1
    }

    public class MatchingPlayer
    {
        public string playerName { get; set; }
        public IClientProxy client { get; set; }
        public PlayerStatus status { get; set; }
        public string ticketToken { get; set; }
    }
}
