using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(TriggerEvent))]
public class TriggerEventsEditor : Editor
{
    EventManager em;
    TriggerEvent trigEv;
    string newEventName;
    GameObject obj;
    private GUIStyle _titleStyle;
    int selected;
    string newName;

    private void OnEnable()
    {
        trigEv = (TriggerEvent)target;

        _titleStyle = new GUIStyle();
        _titleStyle.fontStyle = FontStyle.Bold;
        _titleStyle.alignment = TextAnchor.MiddleCenter;


    }

    public bool GetEventManager()
    {
        object[] objs = Object.FindObjectsOfType(typeof(EventManager));
        if (objs.Length != 1)
        {
            return false;
        }
        em = objs[0] as EventManager;
        return true;
    }

    public override void OnInspectorGUI()
    {
        if (!GetEventManager())
        {
            EditorGUILayout.HelpBox("No puede tener mas de un Event Manager en la escena", MessageType.Error);
            return;
        }

        Configuration();
        ShowEvents();

    }

    private void ShowEvents()
    {
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Events", _titleStyle);

        for (int i = 0; i < trigEv.nameEvents.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(trigEv.nameEvents[i]);
            if (GUILayout.Button("Remove"))
            {
                trigEv.RemoveEvent(trigEv.nameEvents[i]);
                break;
            }
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.BeginHorizontal();

        newName = EditorGUILayout.TextField(newName);
        if (GUILayout.Button("Add Event"))
        {
            trigEv.AddEvent(newName);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    private void Configuration()
    {
        EditorGUILayout.LabelField("Configuration", _titleStyle);
        GuiLine();
        EditorGUILayout.Space();

        trigEv._type = (TriggerEvent.Type)EditorGUILayout.EnumPopup("Type: ", trigEv._type);
        LayerMask tempMask = EditorGUILayout.MaskField("Triggerable layers: ",InternalEditorUtility.LayerMaskToConcatenatedLayersMask(trigEv.layersTriggereable), InternalEditorUtility.layers);
        trigEv.layersTriggereable = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(tempMask);
        trigEv.deactivateAfterUse = EditorGUILayout.Toggle("Deactivate after: ", trigEv.deactivateAfterUse);
    }

    void GuiLine(int i_height = 1)

    {

        Rect rect = EditorGUILayout.GetControlRect(false, i_height);

        rect.height = i_height;

        EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));

    }
}
