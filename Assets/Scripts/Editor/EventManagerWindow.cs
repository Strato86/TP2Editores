using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;


public class EventManagerWindow : EditorWindow
{
    string newEventName;
    GameObject obj;
    int selected;
    public static void OpenWindow()
    {
        var w = (EventManagerWindow)GetWindow(typeof(EventManagerWindow));
        
    }


    private void OnGUI()
    {

        EditorGUILayout.LabelField("Seleccione su objeto EventManager");
        obj = (GameObject)EditorGUILayout.ObjectField(obj, typeof(GameObject), true);

        if (obj == null) {
            EditorGUILayout.HelpBox("Selecciona un objeto EventManager", MessageType.Info);
            return;
        }
        EventManager em =obj.GetComponent<EventManager>();
        if (em == null) {
            EditorGUILayout.HelpBox("No es un EventManager", MessageType.Info);
            return;
        }
        em.AddAvailableFunction(PrintHelloWorld);
        em.AddAvailableFunction(PrintHi);
        em.AddAvailableFunction(PrintBye);
        EditorGUILayout.LabelField("Todos los eventos");
        Debug.Log("instance event manager"+ em);
        Debug.Log("instance event manager dic" + em);
         Dictionary<string, List<EventManager.eventFunction>> dic = em.dic;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        foreach (var key in dic.Keys)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Nombre del evento: "+ key);
            if (GUILayout.Button("Delete"))
            {
                em.DeleteEvent(key);
                return;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
           foreach (var value in dic[key]) {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("fun: " + value.Method.Name);
                if (GUILayout.Button("Delete"))
                {
                    em.UnsubscribeEvent(key, value);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }
            selected = EditorGUILayout.Popup("New function",selected, em.NamesAvailableFunction());
            if (GUILayout.Button("Add function"))
            {
                 em.SubscribeEvent(key, em.GetFunctionId(selected));
                return;
            }
        }



        Rect rect = EditorGUILayout.GetControlRect(false, 1f);
        rect.height = 1f;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

        EditorGUILayout.LabelField("Create new event");
        EditorGUILayout.BeginHorizontal();
        newEventName = EditorGUILayout.TextField(newEventName);
        if (GUILayout.Button("Create"))
        {
            em.AddEvent(newEventName);
        }
        EditorGUILayout.EndHorizontal();
    }

    private void PrintBye(object[] parameterContainer)
    {
        Debug.Log("Bye");
    }

    private void PrintHi(object[] parameterContainer)
    {
        Debug.Log("Hi");
    }

    private void PrintHelloWorld(object[] parameterContainer)
    {
        Debug.Log("HelloWorld");
    }


}
