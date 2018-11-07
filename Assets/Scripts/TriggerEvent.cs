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

    public void GetEventManager()
    {
        object[] objs = UnityEngine.Object.FindObjectsOfType(typeof(EventManager));
        if (objs.Length != 1)
        {
            eventManager = null;
        }
        eventManager = objs[0] as EventManager;

    }

    private void Start()
    {
        GetEventManager();
    }
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
        if (_type == Type.Enter && ((1 << other.gameObject.layer) & layersTriggereable) != 0) {
            ExecuteAllEvents();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (_type == Type.Stay && ((1 << other.gameObject.layer) & layersTriggereable) != 0)
        {
            ExecuteAllEvents();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_type == Type.Exit && ((1 << other.gameObject.layer) & layersTriggereable) != 0)
        {
            ExecuteAllEvents();
        }
    }

    private void ExecuteAllEvents()
    {
        if (eventManager == null) {
            print("ERROR: No tenes un Event Manager en escena, no se van a ejecutar los eventos");
            return;
        }
        foreach (var name in nameEvents)
        {
            eventManager.ExecuteEvent(name);
        }
        if (deactivateAfterUse) {
            this.gameObject.SetActive(false);
        }
    }
}
