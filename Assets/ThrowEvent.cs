using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEvent : MonoBehaviour {
    List<string> eventos = new List<string>();
    EventManager em;
    private void OnTriggerEnter(Collider other)
    {
        em.ExecuteEvent(eventos);
    }
    public void addEvent(string name) {
        eventos.Add(name);

    }


    public bool GetEventManager()
    {
        object[] objs = Object.FindObjectsOfType(typeof(EventManager));
        if (objs.Length!= 1) {
            return false;
        }
        em = objs[0] as EventManager;
        return true;
    }
}
