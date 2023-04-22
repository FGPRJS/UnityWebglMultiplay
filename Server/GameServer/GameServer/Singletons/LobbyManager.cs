using GameServer.Model;
using System.Collections.Concurrent;
using System.Text.Json;
using GameServer.Hub.Lobby;

namespace GameServer.Singletons
{
    public class LobbyManager
    {
        private const int LobbySize = 2;
        private const int TicketSize = 16;
        
        private readonly ConcurrentQueue<MatchingPlayer> _ticketQueue;
        private readonly ConcurrentDictionary<string, MatchingPlayer> _ticketTokens;

        private List<MatchingPlayer> _currentLobby;

        private readonly Random _random;
        private readonly Timer _queueMatchTimer;
        private QueueMatchStatus _matchStatus;

        public LobbyManager()
        {
            this._ticketQueue = new ConcurrentQueue<MatchingPlayer>();
            this._ticketTokens = new ConcurrentDictionary<string, MatchingPlayer>();

            this._currentLobby = new List<MatchingPlayer>(LobbySize);

            this._queueMatchTimer = new Timer((ev) =>
            {
                this.TicketMatching();
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));

            this._random = new Random();
        }

        private string GenerateRandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[this._random.Next(s.Length)])
                    .ToArray());
            return result;
        }

        public string IssueTicket(MatchingPlayer lobbyPlayer)
        {
            var ticketToken = GenerateRandomString(TicketSize);

            while (this._ticketTokens.ContainsKey(ticketToken) == true)
            {
                ticketToken = GenerateRandomString(TicketSize);
            }

            lobbyPlayer.ticketToken = ticketToken;

            this._ticketQueue.Enqueue(lobbyPlayer);
            this._ticketTokens.TryAdd(ticketToken, lobbyPlayer);

            return ticketToken;
        }

        public void CancelTicket(string ticketToken)
        {
            if (!this._ticketTokens.ContainsKey(ticketToken))
            {
                return;
            }

            this._ticketTokens.TryRemove(ticketToken, out var ticketPlayer);

            ticketPlayer.status = PlayerStatus.Canceled;
        }

        private void TicketMatching()
        {
            if (this._matchStatus == QueueMatchStatus.Busy)
            {
                return;
            }

            this._matchStatus = QueueMatchStatus.Busy;

            #region Remove Canceled Player in lobby

            this._currentLobby = this._currentLobby.Where(
                player => player.status == PlayerStatus.Matching)
                .ToList();

            #endregion

            while (this._ticketQueue.IsEmpty == false)
            {
                this._ticketQueue.TryDequeue(out var player);

                if (player == null)
                {
                    continue;
                }

                if (this._ticketTokens.ContainsKey(player.ticketToken) == false)
                {
                    continue;
                }

                if (player.status != PlayerStatus.Matching)
                {
                    continue;
                }

                switch (this._currentLobby.Count)
                {
                    case < LobbySize - 1:
                        this._currentLobby.Add(player);
                        continue;
                    case LobbySize - 1:
                    
                        var newMatchedInfos = new List<MatchedInfo>
                        {
                            new()
                            {
                                playerId = player.playerName
                            }
                        };

                        newMatchedInfos.AddRange(this._currentLobby.Select(
                            matchedPlayer => new MatchedInfo()
                            {
                                playerId = matchedPlayer.playerName
                            }));

                        var matchInfoJson = JsonSerializer.Serialize(newMatchedInfos);

                        foreach (var matchedPlayer in this._currentLobby)
                        {
                            this._ticketTokens.TryRemove(matchedPlayer.ticketToken, out _);
                            matchedPlayer.client.SendCoreAsync(LobbyMethod.TicketMatched, new object[] { matchInfoJson });
                        }

                        this._currentLobby = new List<MatchingPlayer>(LobbySize);

                        break;
                    
                    case >= LobbySize:

                        newMatchedInfos = new List<MatchedInfo>(LobbySize);

                        newMatchedInfos.AddRange(this._currentLobby.GetRange(0, LobbySize).Select(
                            matchedPlayer => new MatchedInfo()
                            {
                                playerId = matchedPlayer.playerName
                            }));

                        matchInfoJson = JsonSerializer.Serialize(newMatchedInfos);

                        foreach (var matchedPlayer in this._currentLobby)
                        {
                            this._ticketTokens.TryRemove(matchedPlayer.ticketToken, out _);
                            matchedPlayer.client.SendCoreAsync(LobbyMethod.TicketMatched, new object[] { matchInfoJson });
                        }

                        this._currentLobby = this._currentLobby.GetRange(LobbySize, this._currentLobby.Count - LobbySize);

                        break;
                }
            }

            this._matchStatus = QueueMatchStatus.Idle;
        }
    }
}
