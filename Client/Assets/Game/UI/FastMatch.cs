using System;
using Game.Singleton;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class FastMatch : MonoBehaviour
    {
        public TMP_Text buttonText;

        public void ButtonAction()
        {
            if (ConnectionManager.Instance.ConnectionStatus == ConnectionStatus.NotConnected)
            {
                LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                {
                    log = "Reconnecting..."
                });
                ConnectionManager.Instance.ConnectToServer();
                return;
            }
            
            switch (ConnectionManager.Instance.MatchingStatus)
            {
                case MatchingStatus.Idle:
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Matching..."
                    });
                    ConnectionManager.Instance.IssueTicket();
                    
                    break;
                
                case MatchingStatus.Matching:
                    LogMessageManager.instance.logEvent.Invoke(new LogEventData()
                    {
                        log = "Match canceled."
                    });
                    ConnectionManager.Instance.CancelTicket();
                    break;
                
                case MatchingStatus.Matched:
                default:
                    throw new ArgumentOutOfRangeException();
            }
           
        }

        private void Awake()
        {
            ConnectionManager.Instance.ConnectionStatusChangedEvent.AddListener((e) =>
            {
                switch (e.status)
                {
                    case ConnectionStatus.NotConnected:

                        this.buttonText.text = "RECONNECT";
                        return;
                        
                    case ConnectionStatus.Connected:
                    default:
                        break;
                }
            });
            
            ConnectionManager.Instance.MatchingStatusChangedEvent.AddListener((e) =>
            {
                switch (e.status)
                {
                    case MatchingStatus.Idle:

                        this.buttonText.text = "MATCH";
                        
                        break;
                    case MatchingStatus.Matching:

                        this.buttonText.text = "CANCEL MATCH";
                        
                        break;
                    case MatchingStatus.Matched:

                        this.buttonText.text = "MATCHED";

                        break;
                    default:
                        break;
                }
            });
        }

        // Start is called before the first frame update
        void Start()
        {
            switch (ConnectionManager.Instance.ConnectionStatus)
            {
                case ConnectionStatus.NotConnected:

                    this.buttonText.text = "RECONNECT";
                    return;

                case ConnectionStatus.Connected:
                default:
                    break;
            }
            
            switch (ConnectionManager.Instance.MatchingStatus)
            {
                case MatchingStatus.Idle:

                    this.buttonText.text = "MATCH";
                        
                    break;
                case MatchingStatus.Matching:

                    this.buttonText.text = "CANCEL MATCH";
                        
                    break;
                case MatchingStatus.Matched:

                    this.buttonText.text = "MATCHED";

                    break;
                default:
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
