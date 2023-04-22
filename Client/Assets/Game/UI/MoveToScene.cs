using System.Collections;
using Game.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public class MoveToScene : MonoBehaviour
    {
        void Start()
        {
            GameSceneManager.Instance.gameSceneLoadingEvent.AddListener(
                (e) =>
                {
                    LogMessageManager.instance.logEvent.Invoke(
                        new LogEventData()
                        {
                            log = e.message
                        });
                } );
        }

        public void MoveToLobbyScene()
        {
            GameSceneManager.Instance.MoveToScene(SceneName.LobbyScene);
        }
    }
}
