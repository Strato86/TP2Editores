using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class StartInterface : EditorWindow
{
    public GameObject CurEventMan;
    private string _quesIconFilename;

    [MenuItem("Tools/Level Editor")]
    public static void OpenWindow()
    {
        var self = (StartInterface)StartInterface.GetWindow(typeof(StartInterface), true, "Level Editor");
        self.position = new Rect(500, 250, 250, 250);
    }


    private void OnGUI()
    {
        Features();
    }

    private void Features()
    {
        maxSize = new Vector2(250, 250);
        minSize = new Vector2(250, 250);

        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Level Editor", EditorStyles.boldLabel);

        _quesIconFilename = "questionmark";
        GUI.DrawTexture(GUILayoutUtility.GetRect(15, 15), (Texture2D)Resources.Load(_quesIconFilename), ScaleMode.ScaleToFit);
        if (new Rect(227.5f, 3f, 20, 20).Contains(Event.current.mousePosition))
        {
            Repaint();
            EditorGUI.DrawRect(new Rect(230, 6, 15, 15), new Color(0.9f, 0.9f, 0.9f, 0.7f));
        }


        var margin = new Vector2(235, 0);
        var isPressed = false;
        isPressed = Handles.Button(GUILayoutUtility.GetLastRect().position + margin, Quaternion.identity, 15, 15, Handles.CubeHandleCap);
        if (isPressed) { HowToUse.OpenWindow(); }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        EditorGUI.DrawRect(GUILayoutUtility.GetRect(100, 1), Color.gray);

        GUILayout.BeginArea(new Rect(25, 32, Screen.width, Screen.height - 30));
        {
            if (GUILayout.Button("New Level", GUILayout.Height(40), GUILayout.Width(200)))
            {
                CustomRoomWindow.OpenWindow(0, Vector2Int.zero, Vector2Int.zero);
            }

            if (GUILayout.Button("Load Level", GUILayout.Height(40), GUILayout.Width(200)))
            {
                ItemSelectionInterface.OpenWindow();
                /*string path = EditorUtility.OpenFilePanel("Load Level", "Assets/Levels", "asset");
                if (path.Length != 0)
                {
                    //var fileContent = File.ReadAllBytes(path);
                    //texture.LoadImage(fileContent);
                    //CustomRoomWindow.OpenWindow(0, Vector2Int.zero, Vector2Int.zero);
                }*/
            }

            if (GUILayout.Button("Event Manager", GUILayout.Height(40), GUILayout.Width(200)))
            {
                if (GameObject.Find("EventManager") == null)
                {
                    EventManCreator();
                }
                else Selection.activeGameObject = GameObject.Find("EventManager");
            }

            GUILayout.BeginArea(new Rect(0, 160, 200, 40));
            {
                if (GUILayout.Button("Close", GUILayout.Height(40), GUILayout.Width(200))) Close();
            }
            GUILayout.EndArea();
        }
        GUILayout.EndArea();
    }

    public void EventManCreator()
    {
        GameObject EventMan;
        EventMan = new GameObject("EventManager");
        EventMan.AddComponent<EventManager>();
        CurEventMan = EventMan;
    }

/*    private void ScreenShot()
    {
        var width = 950;
        var height = 630;
        var startX = 270;
        var startY = 10;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rex = new Rect(startX, startY, width, height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        var bytes = tex.EncodeToPNG();
        DestroyImmediate(tex);

        System.IO.File.WriteAllBytes(Application.dataPath + "/Resources/Previews/" + saveName + ".jpg", bytes);

    }*/

}


/*To do
 
  -Making it so it loads an already existing map

  */