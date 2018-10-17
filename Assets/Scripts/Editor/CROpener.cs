using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CROpener : EditorWindow {


    [MenuItem("/Tools/Level Editor Opener")]
    public static void OpenWindow()
    {
        var w = (CROpener)GetWindow(typeof(CROpener));
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Open Editor"))
        {
            CustomRoomWindow.OpenWindow(0, Vector2Int.zero, Vector2Int.zero);
        }
    }
}
