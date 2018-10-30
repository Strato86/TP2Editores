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



        EditorGUILayout.LabelField("Todos los eventos", _titleStyle);
        Debug.Log("instance event manager" + em);
        Debug.Log("instance event manager dic" + em);


        EditorGUILayout.Space();
        EditorGUILayout.Space();
        for (int i = 0; i < em.eventsNames.Count; i++)

        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Evento: " + em.eventsNames[i],_titleStyle);
            if (GUILayout.Button("Delete"))
            {
                em.DeleteEvent(em.eventsNames[i]);
                return;
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();


            this.serializedObject.Update();

            var a = this.serializedObject.FindProperty("events");

            EditorGUILayout.PropertyField(a.GetArrayElementAtIndex(i), true);
            this.serializedObject.ApplyModifiedProperties();


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
