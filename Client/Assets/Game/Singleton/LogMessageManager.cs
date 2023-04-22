using UnityEngine;

namespace Game.Singleton
{
    public class LogMessageManager : MonoBehaviour
    {
        public static LogMessageManager instance { get; private set; }

        public LogEvent logEvent;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            
                DontDestroyOnLoad(this);
            }
        
            logEvent = new LogEvent();
        }
    }
}
