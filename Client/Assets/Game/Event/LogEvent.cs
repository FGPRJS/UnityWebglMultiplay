using UnityEngine.Events;

public class LogEventData
{
    public string log { get; set; }
}

public class LogEvent : UnityEvent<LogEventData>
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
