using System;
using Plugins.SignalR;
using UnityEngine;

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

        private SignalR _signalRHub;

        private string _currentTicket;
    
        public void ConnectToServer()
        {
            try{
                // Initialize SignalR
                _signalRHub = new SignalR();
                _signalRHub.Init("http://localhost:5000/lobby");

                _signalRHub.SignalRMessageEvent += (message) =>
                {
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = message
                    });
                };
                
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
                LogMessageManager.instance.logEvent.Invoke(
                    new LogEventData()
                    {
                        log = "Failed to connect to server."
                    });
            }
        }

        public void CancelTicket()
        {
            
        }

        public void IssueTicket()
        {
            this._signalRHub.Invoke(
                LobbyMethod.IssueTicket, 
                GameManager.Instance.playerInfo.playerLoginInfo.playerName);
        }

        // Update is called once per frame
        void Start()
        {
            ConnectToServer();
        }
    }
}
