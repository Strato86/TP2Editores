using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

[CustomEditor(typeof(EventManager))]
public class EventManagerEditor : Editor
{
    EventManager em;
    string newEventName;
    GameObject obj;
    private GUIStyle _titleStyle;
    int selected;
    public UnityEvent myEvent;
    private void OnEnable()
    {
        em = (EventManager)target;

        _titleStyle = new GUIStyle();
        _titleStyle.fontStyle = FontStyle.Bold;
        _titleStyle.alignment = TextAnchor.MiddleCenter;


    }


    public override void OnInspectorGUI()
    {


        em.AddAvailableFunction(PrintHelloWorld);
        em.AddAvailableFunction(PrintHi);
        em.AddAvailableFunction(PrintBye);
        EditorGUILayout.LabelField("Todos los eventos", _titleStyle);
        Debug.Log("instance event manager" + em);
        Debug.Log("instance event manager dic" + em);
        Dictionary<string, List<EventManager.eventFunction>> dic = em.dic;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        foreach (var key in dic.Keys)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Nombre del evento: " + key);
            if (GUILayout.Button("Delete"))
            {
                em.DeleteEvent(key);
                return;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            foreach (var value in dic[key])
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("fun: " + value.Method.Name);
                if (GUILayout.Button("Delete"))
                {
                    em.UnsubscribeEvent(key, value);
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            this.serializedObject.Update();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onEvent"), true);
            this.serializedObject.ApplyModifiedProperties();

            if (GUILayout.Button("Add function"))
            {
                Debug.Log(this.serializedObject.FindProperty("onEvent"));
                em.SubscribeEvent(key, em.GetFunctionId(selected));
                return;
            }
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        Rect rect = EditorGUILayout.GetControlRect(false, 1f);
        rect.height = 1f;
        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Create new event",_titleStyle);
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
