using System.Collections.Concurrent;

namespace GameServer.Singletons
{
    public class Game
    {

    }

    public class GameManager
    {
        private Random _random;

        /// <summary>
        /// Key : Room Number
        /// Value : Game Obj
        /// </summary>
        private ConcurrentDictionary<string, Game> _games;

        public GameManager()
        {
            this._random = new Random();

            this._games = new ConcurrentDictionary<string, Game>();
        }
    }
}
