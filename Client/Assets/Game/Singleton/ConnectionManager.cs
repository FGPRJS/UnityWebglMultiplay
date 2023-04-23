using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Plugins.SignalR;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Singleton
{
    public static class GameMethod
    {
        public static string EnterGame = "EnterGame";
        public static string PlayerConnected = "PlayerConnected";
    }

    
    public static class LobbyMethod
    {
        public const string IssueTicket = "IssueTicket";
        public const string CancelTicket = "CancelTicket";
        public const string TicketMatched = "TicketMatched";
    }

    #region Connection Status
    public enum ConnectionStatus
    {
        NotConnected,
        Connected
    }
    
    public class ConnectionStatusChangedEventData
    {
        public ConnectionStatus status;
    }
    #endregion

    #region Matching Status
    public enum MatchingStatus
    {
        Idle,
        Matching,
        Matched
    }

    public class MatchingStatusChangedEventData
    {
        public MatchingStatus status;
    }
    
    #endregion
    
    
    public class ConnectionManager : MonoBehaviour
    {
        #region Singleton
        public static ConnectionManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;

                this.ConnectionStatus = ConnectionStatus.NotConnected;
                this.ConnectionStatusChangedEvent = new UnityEvent<ConnectionStatusChangedEventData>();
                
                DontDestroyOnLoad(this);
            }
        }
        
        #endregion
        
        #region ConnectionStatus
        private ConnectionStatus _connectionConnectionStatus;
        public ConnectionStatus ConnectionStatus
        {
            get => this._connectionConnectionStatus;
            set
            {
                this._connectionConnectionStatus = value;
                this.ConnectionStatusChangedEvent.Invoke(new ConnectionStatusChangedEventData()
                {
                    status = this._connectionConnectionStatus
                });
            }
        }
        
        public UnityEvent<ConnectionStatusChangedEventData> ConnectionStatusChangedEvent;
        
        #endregion
        
        #region MatchingStatus

        private MatchingStatus _matchingStatus;

        public MatchingStatus MatchingStatus
        {
            get => this._matchingStatus;
            set
            {
                this._matchingStatus = value;
                this.MatchingStatusChangedEvent.Invoke(new MatchingStatusChangedEventData()
                {
                    status = this._matchingStatus
                });
            }
        }
        
        public UnityEvent<MatchingStatusChangedEventData> MatchingStatusChangedEvent;

        
        #endregion

        private SignalR _signalRHub;
        private SignalR _gameRHub;

        private string _currentTicket;

        public void Initialize()
        {
            _signalRHub = new SignalR();
            _gameRHub = new SignalR();
        }
        
        public void ConnectToLobby()
        {
            try
            {
                GameManager.Instance.currentGameInfo = null;

                // Initialize Signal
                _signalRHub.Init("http://localhost:5000/lobby");

                // Handler callbacks
                _signalRHub.On(LobbyMethod.IssueTicket, (string ticket) =>
                {
                    _currentTicket = ticket;
                    
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Issue ticket. Waiting in queue..."
                    });
                });
                
                _signalRHub.On(LobbyMethod.TicketMatched, (string matchedInfo) =>
                {
                    _currentTicket = null;

                    GameManager.Instance.currentGameInfo = JsonSerializer.Deserialize<GameInfo>(matchedInfo);

                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Matched!"
                    });
                    
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = $"Matched Info : {matchedInfo}"
                    });
                    
                    this.MatchingStatus = MatchingStatus.Idle;
                    this.ConnectionStatus = ConnectionStatus.NotConnected;

                    _signalRHub.Stop();

                    this.ConnectToGame();
                });


                // Connection callbacks
                _signalRHub.ConnectionStarted += (object sender, ConnectionEventArgs e) =>
                {
                    this.ConnectionStatus = ConnectionStatus.Connected;
                    this.MatchingStatus = MatchingStatus.Idle;
                    
                    // Log the connected ID
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Connect to lobby hub complete."
                    });
                };
                _signalRHub.ConnectionClosed += (object sender, ConnectionEventArgs e) =>
                {
                    
                };

                _signalRHub.Connect();
            }
            catch (Exception)
            {
                this._connectionConnectionStatus = ConnectionStatus.NotConnected;
                
                LogMessageManager.instance.logEvent.Invoke(
                    new LogEventData()
                    {
                        log = "Failed to connect to lobby server."
                    });
            }
        }

        public void ConnectToGame()
        {
            try{
                _gameRHub.Init("http://localhost:5000/game");

                _gameRHub.On(GameMethod.EnterGame, (string currentGamePlayInfoJson) =>
                {
                    var currentGamePlayInfo = JsonSerializer.Deserialize<GamePlayInfo>(
                        currentGamePlayInfoJson);

                    GameManager.Instance.currentGamePlayInfo = currentGamePlayInfo;

                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Enter game successful."
                    });
                });
                
                _signalRHub.On(GameMethod.PlayerConnected, (string gameId, string playerName) =>
                {
                    
                });
                
                // Connection callbacks
                _gameRHub.ConnectionStarted += (object sender, ConnectionEventArgs e) =>
                {
                    // Log the connected ID
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Connect to game hub complete."
                    });
                    
                    GameSceneManager.Instance.MoveToScene(SceneName.GameScene);
                    
                    _gameRHub.Invoke(
                        GameMethod.EnterGame, 
                        GameManager.Instance.currentGameInfo.gameId, 
                        GameManager.Instance.playerInfo.playerLoginInfo.playerName);
                };
                _gameRHub.ConnectionClosed += (object sender, ConnectionEventArgs e) =>
                {
                    
                };

                _gameRHub.Connect();
            }
            catch (Exception)
            {
                LogMessageManager.instance.logEvent.Invoke(
                    new LogEventData()
                    {
                        log = "Failed to connect to game server."
                    });
            }
        }

        public void CancelTicket()
        {
            this._signalRHub.Invoke(
                LobbyMethod.CancelTicket, 
                this._currentTicket);
            
            this.MatchingStatus = MatchingStatus.Idle;
        }

        public void IssueTicket()
        {
            this._signalRHub.Invoke(
                LobbyMethod.IssueTicket, 
                GameManager.Instance.playerInfo.playerLoginInfo.playerName);
            
            this.MatchingStatus = MatchingStatus.Matching;
        }

        // Update is called once per frame
        void Start()
        {
            Initialize();
            ConnectToLobby();
        }

        void OnApplicationQuit()
        {
            if (MatchingStatus == MatchingStatus.Matching)
            {
                ConnectionManager.Instance.CancelTicket();
            }
        }
    }
}
