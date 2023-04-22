using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Game.Singleton
{
    public static class SceneName
    {
        public const string LoginScene = "LoginScene";
        public const string LobbyScene = "LobbyScene";
        public const string GameScene = "GameScene";
    }

    public class GameSceneEventData
    {
        public string message { get; set; }
    }
    
    public class GameSceneManager : MonoBehaviour
    {
        public UnityEvent<GameSceneEventData> gameSceneLoadingEvent;
        public static GameSceneManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                this.gameSceneLoadingEvent = new UnityEvent<GameSceneEventData>();
                
                Instance = this;

                DontDestroyOnLoad(this);
            }
        }

        public void MoveToScene(string sceneName)
        {
            this.gameSceneLoadingEvent.Invoke(new GameSceneEventData()
            {
                message = $"Loading scene..."
            });
            
            StartCoroutine(LoadScene(sceneName));
        }

        IEnumerator LoadScene(string sceneName)
        {
            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                
                this.gameSceneLoadingEvent.Invoke(new GameSceneEventData()
                {
                    message = $"Loading scene process : {asyncLoad.progress * 100}%"
                });

                yield return null;
            }
            
            this.gameSceneLoadingEvent.Invoke(new GameSceneEventData()
            {
                message = $"Loading scene complete."
            });
        }
    }
}