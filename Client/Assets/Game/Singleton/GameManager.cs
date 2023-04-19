using System;
using Game.Data;
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

        public void ChangePlayerName(string newName)
        {
            this.playerInfo.playerLoginInfo.playerName = newName;
        }

        private void OnEnable()
        {
            DontDestroyOnLoad(this);
        }

        void Awake()
        {
            
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
