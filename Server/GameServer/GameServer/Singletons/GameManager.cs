using System.Collections.Concurrent;
using GameServer.Model;

namespace GameServer.Singletons
{
    public class GameInfo
    {
        public Dictionary<string, MatchedInfo> matchedInfos { get; set; }
        public string gameId { get; set; }
    }

    public class GamePlayerInfo
    {

    }

    public class GamePlayInfo
    {
        public Dictionary<string, GamePlayerInfo> gamePlayerInfos { get; set; }
    }

    public class GameManager
    {
        private const int GroupIdLength = 9;

        private readonly RandomManager _randomManager;

        /// <summary>
        /// Key : Group ID
        /// Value : GameInfo Obj
        /// </summary>
        private ConcurrentDictionary<string, GameInfo> _gameInfos;
        private ConcurrentDictionary<string, GamePlayInfo> _gamePlayInfos;

        public GameManager(RandomManager randomManager)
        {
            this._randomManager = randomManager;

            this._gameInfos = new ConcurrentDictionary<string, GameInfo>();
            this._gamePlayInfos = new ConcurrentDictionary<string, GamePlayInfo>();
        }

        public GameInfo CreateGameInfo(Dictionary<string, MatchedInfo> matchedInfos)
        {
            var newGroupId = this._randomManager.GenerateRandomString(GroupIdLength);

            while (this._gameInfos.ContainsKey(newGroupId) == true)
            {
                newGroupId = this._randomManager.GenerateRandomString(GroupIdLength);
            }

            var newGameInfo = new GameInfo()
            {
                gameId = newGroupId,
                matchedInfos = matchedInfos
            };

            this._gameInfos.TryAdd(newGroupId, newGameInfo);

            return newGameInfo;
        }

        public GamePlayInfo EnterGame(string gameId, string playerName)
        {
            this._gamePlayInfos.TryGetValue(gameId, out var gamePlayInfo);

            if (gamePlayInfo == null)
            {
                gamePlayInfo = new GamePlayInfo()
                {
                    gamePlayerInfos = new Dictionary<string, GamePlayerInfo>()
                };

                this._gamePlayInfos.TryAdd(playerName, gamePlayInfo);
            }

            gamePlayInfo.gamePlayerInfos.TryGetValue(playerName, out var gamePlayerInfo);

            if (gamePlayerInfo == null)
            {
                gamePlayerInfo = new GamePlayerInfo()
                {

                };

                gamePlayInfo.gamePlayerInfos.TryAdd(playerName, gamePlayerInfo);
            }

            return gamePlayInfo;
        }
    }
}
