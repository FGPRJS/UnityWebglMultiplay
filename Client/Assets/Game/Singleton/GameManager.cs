using System;
using Game.Data;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

namespace Game.Singleton
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] public PlayerInfo playerInfo;

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
