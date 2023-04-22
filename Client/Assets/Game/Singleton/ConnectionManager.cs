using System;
using Plugins.SignalR;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Singleton
{
    public static class GameHubMethod
    {
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

        private string _currentTicket;
    
        public void ConnectToServer()
        {
            try{
                // Initialize SignalR
                _signalRHub = new SignalR();
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
                    
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Matched!"
                    });
                    
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = $"Matched Info : {matchedInfo}"
                    });
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
                    this.ConnectionStatus = ConnectionStatus.NotConnected;
                };

                _signalRHub.Connect();
            }
            catch (Exception)
            {
                this._connectionConnectionStatus = ConnectionStatus.NotConnected;
                
                LogMessageManager.instance.logEvent.Invoke(
                    new LogEventData()
                    {
                        log = "Failed to connect to server."
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
            ConnectToServer();
        }
    }
}
