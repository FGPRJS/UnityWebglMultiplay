using System;
using System.Collections.Generic;
using Game.Data;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Singleton
{
    public class MatchedInfo
    {
        public string playerId { get; set; }
        public string groupId { get; set; }
    }
    
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
    
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public PlayerInfo playerInfo;
        [SerializeField] public GameInfo currentGameInfo;
        [SerializeField] private GamePlayInfo _currentGamePlayInfo;

        [DoNotSerialize]
        public GamePlayInfo currentGamePlayInfo
        {
            get => this._currentGamePlayInfo;
            set
            {
                this._currentGamePlayInfo = value;
                
                this.GamePlayInfoChangedEvent.Invoke(
                    this._currentGamePlayInfo);
            }
        }
        public UnityEvent<GamePlayInfo> GamePlayInfoChangedEvent;

        public GameManager()
        {
            playerInfo = new PlayerInfo()
            {
                playerLoginInfo = new PlayerLoginInfo()
            };
        }
        
        #region Singleton
        
        public static GameManager Instance { get; private set; }
      
        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                this.GamePlayInfoChangedEvent = new UnityEvent<GamePlayInfo>();
                DontDestroyOnLoad(this);
            }
        }
        
        #endregion
        
        public void ChangePlayerName(string newName)
        {
            this.playerInfo.playerLoginInfo.playerName = newName;
        }

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
