using TMPro;
using Unity.VisualScripting;
using UnityEngine;

namespace Game.UI
{
    public class CurrentPlayerInfoDetail : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playernameText;
        [SerializeField]
        private TMP_Text playerstatusText;
        [SerializeField] private string _playerName;
        [DoNotSerialize]
        public string playerName
        {
            get => this._playerName;
            set
            {
                this._playerName = value;

                this.playernameText.text = this._playerName;
            }
        }

        [SerializeField] private string _playerStatus;

        [DoNotSerialize]
        public string playerStatus
        {
            get => this._playerStatus;
            set
            {
                this._playerStatus = value;

                this.playerstatusText.text = this._playerStatus;
            }
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
