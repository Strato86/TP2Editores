using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour {
    public enum Type {Enter, Stay, Exit }
    public Type _type;
    public List<string> nameEvents = new List<string>();
    public LayerMask layersTriggereable ;
    public bool deactivateAfterUse=false;
    public EventManager eventManager;

    public void AddEvent(string name) {
        if (!nameEvents.Contains(name)) {
            nameEvents.Add(name);
        }
    }

    public void RemoveEvent(string name)
    {
        if (nameEvents.Contains(name))
        {
            nameEvents.Remove(name);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_type == Type.Enter) {
            ExecuteAllEvents();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_type == Type.Enter)
        {
            ExecuteAllEvents();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_type == Type.Enter)
        {
            ExecuteAllEvents();
        }
    }

    private void ExecuteAllEvents()
    {
        foreach (var name in nameEvents)
        {
            eventManager.ExecuteEvent(name);
        }
        if (deactivateAfterUse) {
            this.gameObject.SetActive(false);
        }
    }
}
