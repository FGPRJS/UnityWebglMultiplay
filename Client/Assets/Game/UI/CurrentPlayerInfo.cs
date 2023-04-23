using Game.Singleton;
using UnityEngine;

namespace Game.UI
{
    public class CurrentPlayerInfo : MonoBehaviour
    {
        [SerializeField]
        public CurrentPlayerInfoDetail detailData;

        void Awake()
        {
            GameManager.Instance.GamePlayInfoChangedEvent.AddListener(
                (gamePlayInfo) =>
                {
                    foreach (Transform child in this.gameObject.transform)
                    {
                        GameObject.Destroy(child.gameObject);
                    }
                    
                    foreach (var gamePlay in gamePlayInfo.gamePlayerInfos)
                    {
                       var refreshed = Instantiate(detailData, this.transform);

                       refreshed.playerName = gamePlay.Key;
                       refreshed.playerStatus = "Connected";
                    }
                });
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
