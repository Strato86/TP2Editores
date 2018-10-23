using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class EventManagerWindow : EditorWindow
{
    GameObject obj;
    public static void OpenWindow()
    {
        var w = (EventManagerWindow)GetWindow(typeof(EventManagerWindow));
        
    }


    private void OnGUI()
    {
        EditorGUILayout.LabelField("Seleccione su objeto EventManager");
        obj = (GameObject)EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
        EventManager em =obj.GetComponent<EventManager>();
        if (em == null) {
            EditorGUILayout.HelpBox("Selecciona un objeto EventManager", MessageType.Info);
            return;

        }
        EditorGUILayout.LabelField("Todos los eventos");
        Debug.Log("instance event manager"+ em);
        Debug.Log("instance event manager dic" + em);
         Dictionary<string, List<EventManager.eventFunction>> dic = em.dic;
        em.SubscribeEvent("lala", PrintHelloWorld);
        foreach (var key in dic.Keys)
        {
            EditorGUILayout.LabelField("Nombre del evento: "+ key);
            foreach (var value in dic[key]) {
                EditorGUILayout.LabelField("fun: " + value.Method.Name);

            }
        }
    }

    private void PrintHelloWorld(object[] parameterContainer)
    {
        Debug.Log("HelloWorld");
    }

    public void PrintHi()
    {
        Debug.Log("Hi");
    }
    public void PrintBye()
    {
        Debug.Log("Bye");
    }
}
