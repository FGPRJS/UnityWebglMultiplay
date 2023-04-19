using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityEventManager : MonoBehaviour
{
    public static UnityEventManager instance { get; private set; }

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
