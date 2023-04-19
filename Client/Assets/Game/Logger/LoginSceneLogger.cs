using System.Collections.Generic;
using UnityEngine;

namespace Game.Logger
{
    public class LoginSceneLogger : MonoBehaviour
    {
        [SerializeField]
        public List<string> LoginSceneLog;

        [SerializeField]
        public GameObject LogContainer;

        [SerializeField]
        public GameLog LogObject;
        
        void AddLog(LogEventData data)
        {
            this.LoginSceneLog.Add(data.log);

            var instantiatedLogObject = Instantiate<GameLog>(
                LogObject, 
                LogContainer.transform, 
                true);

            instantiatedLogObject.Log = data.log;
        }

        private void Awake()
        {
            UnityEventManager.instance.logEvent.AddListener(AddLog);
        }
    }
}
