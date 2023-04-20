using System;
using UnityEngine;

namespace Game.Singleton
{
    public static class GameHubMethod
    {
        public static string PlayerConnected = "PlayerConnected";
    }
    
    public class ConnectionManager : MonoBehaviour
    {
        #region Singleton
        public static ConnectionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                
                DontDestroyOnLoad(this);
            }
        }
        
        #endregion

        private SignalR _gameHub;
    
        // Start is called before the first frame update
        public void ConnectToGame()
        {
            // Initialize SignalR
            _gameHub = new SignalR();
            _gameHub.Init("http://localhost:5000/game");

            // Handler callbacks
            _gameHub.On(GameHubMethod.PlayerConnected, (string payload) =>
            {
                
            });


            // Connection callbacks
            _gameHub.ConnectionStarted += (object sender, ConnectionEventArgs e) =>
            {
                // Log the connected ID
                UnityEventManager.instance.logEvent.Invoke(new LogEventData()
                {
                    log = "Connect to game hub complete."
                });

                _gameHub.Invoke(GameHubMethod.PlayerConnected,
                    GameManager.Instance.playerInfo.playerLoginInfo.playerName);
            };
            _gameHub.ConnectionClosed += (object sender, ConnectionEventArgs e) =>
            {
                
            };

            _gameHub.Connect();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
