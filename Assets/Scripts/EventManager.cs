using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class EventManager  : MonoBehaviour{


    [Serializable]
    public class EventHandler : UnityEvent
    {
    }

    public EventHandler onEvent = new EventHandler();

    [SerializeField]
    public List< EventHandler> events = new List<EventHandler>();
    public List<string> eventsNames = new List<string>();


    public void AddEvent(string name) {
        if (!eventsNames.Contains(name)) {
            events.Add(new EventHandler());
            eventsNames.Add(name);
        }

    }
    public void DeleteEvent(string name)
    {
        if (eventsNames.Contains(name))
        {
            int i=eventsNames.IndexOf(name);
            eventsNames.RemoveAt(i);
            events.RemoveAt(i);
        }
    }



    public void ExecuteEvent(string name)
    {
        if (eventsNames.Contains(name))
        {
            int i = eventsNames.IndexOf(name);
            events[i].Invoke();
        }
        else {
            Debug.LogWarning("NO existe el evento que se esta tratando de ejecutar: "+ name);
        }
    }

}
