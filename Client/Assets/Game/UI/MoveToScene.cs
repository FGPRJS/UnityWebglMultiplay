using System.Collections;
using Game.Singleton;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI
{
    public static class SceneName
    {
        public const string LoginScene = "LoginScene";
        public const string GameScene = "GameScene";
    }

    
    public class MoveToScene : MonoBehaviour
    {
        public void ConnectToServer()
        {
            ConnectionManager.Instance.ConnectToGame();
        }
        
        public void MoveToGameScene()
        {
            UnityEventManager.instance.logEvent.Invoke(new LogEventData()
            {
                log = $"Loading scene..."
            });
            
            StartCoroutine(LoadGameScene());
        }
        
        IEnumerator LoadGameScene()
        {
            var asyncLoad = SceneManager.LoadSceneAsync(SceneName.GameScene);

            while (!asyncLoad.isDone)
            {
                
                UnityEventManager.instance.logEvent.Invoke(new LogEventData()
                {
                    log = $"Loading scene process : {asyncLoad.progress * 100}%"
                });

                yield return null;
            }
            
            UnityEventManager.instance.logEvent.Invoke(new LogEventData()
            {
                log = $"Loading scene complete."
            });
        }
    }
}
