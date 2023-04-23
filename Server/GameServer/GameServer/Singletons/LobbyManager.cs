using GameServer.Model;
using System.Collections.Concurrent;
using System.Text.Json;
using GameServer.Hub.Lobby;

namespace GameServer.Singletons
{
    public class LobbyManager
    {
        private const int LobbySize = 4;
        private const int TicketSize = 16;
        private const int LobbyMaxWaitingSec = 1;
        
        private readonly ConcurrentQueue<MatchingPlayer> _ticketQueue;
        private readonly ConcurrentDictionary<string, MatchingPlayer> _ticketTokens;

        private List<MatchingPlayer> _currentLobby;
        private long _lobbyCreatedTime;

        private readonly Random _random;
        private readonly Timer _queueMatchTimer;
        private QueueMatchStatus _matchStatus;

        private readonly GameManager _gameManager;
        private readonly RandomManager _randomManager;

        public LobbyManager(GameManager gameManager, RandomManager randomManager)
        {
            this._gameManager = gameManager;
            this._randomManager = randomManager;

            this._ticketQueue = new ConcurrentQueue<MatchingPlayer>();
            this._ticketTokens = new ConcurrentDictionary<string, MatchingPlayer>();

            this._currentLobby = new List<MatchingPlayer>(LobbySize);

            this._queueMatchTimer = new Timer((ev) =>
            {
                this.TicketMatching();
            }, null, TimeSpan.Zero, TimeSpan.FromMilliseconds(1000));

            this._random = new Random();

            this._matchStatus = QueueMatchStatus.Idle;
        }

       

        public string IssueTicket(MatchingPlayer lobbyPlayer)
        {
            var ticketToken = this._randomManager.GenerateRandomString(TicketSize);

            while (this._ticketTokens.ContainsKey(ticketToken) == true)
            {
                ticketToken = this._randomManager.GenerateRandomString(TicketSize);
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

            if (ticketPlayer != null)
            {
                ticketPlayer.status = PlayerStatus.Canceled;
            }
        }

        private void TicketMatching()
        {
            if (this._matchStatus == QueueMatchStatus.Busy)
            {
                return;
            }

            this._matchStatus = QueueMatchStatus.Busy;
            Console.WriteLine($"{DateTime.Now} : Lock match status.");

            try
            {

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

                            var currentTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                            if (currentTime - _lobbyCreatedTime < LobbyMaxWaitingSec * 1000)
                            {
                                continue;
                            }

                            var newMatchedInfos = this._currentLobby.Select(
                                matchedPlayer => new MatchedInfo()
                                {
                                    playerName = matchedPlayer.playerName
                                }).ToDictionary((matchedPlayer) => matchedPlayer.playerName);

                            var gameInfo = this._gameManager.CreateGameInfo(newMatchedInfos);

                            var matchInfoJson = JsonSerializer.Serialize(gameInfo);

                            foreach (var matchedPlayer in this._currentLobby)
                            {
                                this._ticketTokens.TryRemove(matchedPlayer.ticketToken, out _);
                                matchedPlayer.client.SendCoreAsync(LobbyMethod.TicketMatched,
                                    new object[] { matchInfoJson });
                            }

                            this._currentLobby = new List<MatchingPlayer>(LobbySize);
                            _lobbyCreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                            continue;
                        case LobbySize - 1:

                            newMatchedInfos = new Dictionary<string, MatchedInfo>
                            {
                                [player.playerName] = new()
                                {
                                    playerName = player.playerName
                                }
                            };

                            foreach (var matchedPlayer in this._currentLobby)
                            {
                                newMatchedInfos.Add(matchedPlayer.playerName, new MatchedInfo()
                                {
                                    playerName = matchedPlayer.playerName
                                });
                            }


                            gameInfo = this._gameManager.CreateGameInfo(newMatchedInfos);

                            matchInfoJson = JsonSerializer.Serialize(gameInfo);

                            foreach (var matchedPlayer in this._currentLobby)
                            {
                                this._ticketTokens.TryRemove(matchedPlayer.ticketToken, out _);
                                matchedPlayer.client.SendCoreAsync(LobbyMethod.TicketMatched,
                                    new object[] { matchInfoJson });
                            }

                            this._currentLobby = new List<MatchingPlayer>(LobbySize);
                            _lobbyCreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                            break;

                        case >= LobbySize:

                            newMatchedInfos = new Dictionary<string, MatchedInfo>
                            {
                                [player.playerName] = new()
                                {
                                    playerName = player.playerName
                                }
                            };

                            foreach (var matchedPlayer in this._currentLobby)
                            {
                                newMatchedInfos.Add(matchedPlayer.playerName, new MatchedInfo()
                                {
                                    playerName = matchedPlayer.playerName
                                });
                            }

                            gameInfo = this._gameManager.CreateGameInfo(newMatchedInfos);

                            matchInfoJson = JsonSerializer.Serialize(gameInfo);

                            foreach (var matchedPlayer in this._currentLobby)
                            {
                                this._ticketTokens.TryRemove(matchedPlayer.ticketToken, out _);
                                matchedPlayer.client.SendCoreAsync(LobbyMethod.TicketMatched,
                                    new object[] { matchInfoJson });
                            }

                            this._currentLobby =
                                this._currentLobby.GetRange(LobbySize, this._currentLobby.Count - LobbySize);
                            _lobbyCreatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                            break;
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                this._matchStatus = QueueMatchStatus.Idle;
                Console.WriteLine($"{DateTime.Now} : Unlock match status.");
            }
        }
    }
}
