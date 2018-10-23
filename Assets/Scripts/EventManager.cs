using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager  : MonoBehaviour{
    public delegate void eventFunction(params object[] parameterContainer);
    public Dictionary<string , List<eventFunction>> dic= new Dictionary<string, List<eventFunction>>();
    public static EventManager instance=null;

    void Awake()
    { 
        if (instance == null) {
            instance = this;
        }
    }


    private void AddEvent(string name) {
        if (!dic.ContainsKey(name)) {
            List<eventFunction> value = new List<eventFunction>();
            dic.Add(name, value);
        }
    }
    private void DeleteEvent(string name)
    {
        if (dic.ContainsKey(name))
        {
            dic.Remove(name);
        }
    }
    public bool HasAFunction(string name, eventFunction function)
    {
        if (!dic.ContainsKey(name))
        {
            return false;
        }
        if (!dic[name].Contains(function))
        {
            return false;
        }
        return true; 
    }
    public void SubscribeEvent(string name, eventFunction function)
    {
        if (!dic.ContainsKey(name)) {
            AddEvent(name);
        }
        if (!dic[name].Contains(function)) {
            dic[name].Add(function);
        }
        
    }
    public void UnsubscribeEvent(string name, eventFunction function)
    {
        if (dic.ContainsKey(name))
        {
            if (dic[name].Contains(function))
                dic[name].Remove(function);
        }
    }

    public void ExecuteEvent(string name)
    {
        if (dic.ContainsKey(name))
        {
            foreach (eventFunction fun in dic[name])
            {
                fun();
            }
        }
        else {
            Debug.LogWarning("NO existe el evento que se esta tratando de ejecutar: "+ name);
        }
    }
    public void ExecuteEvent(string name, params object[] parametersWrapper)
    {
        if (dic.ContainsKey(name))
        {
            foreach (eventFunction fun in dic[name])
            {
                if (parametersWrapper != null) fun(parametersWrapper);
                else fun();
            }
        }
        else
        {
            Debug.LogWarning("NO existe el evento que se esta tratando de ejecutar: " + name);
        }
    }
}
