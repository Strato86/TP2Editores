using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class StartInterface : EditorWindow
{
    [MenuItem("Tools/Level Editor")]
    public static void OpenWindow()
    {
        var self = (StartInterface)StartInterface.GetWindow(typeof(StartInterface), true, "Level Editor");
        self.position = new Rect(500 , 250, 250, 250);
        ItemSelectionInterface.OpenWindow();

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

        GUI.DrawTexture(GUILayoutUtility.GetRect(15, 15), (Texture2D)Resources.Load("eye"), ScaleMode.ScaleToFit);
        var marg2 = new Vector2(230, 0);
        var amPressed = false;
        amPressed = Handles.Button(GUILayoutUtility.GetLastRect().position + marg2, Quaternion.identity, 15, 15, Handles.CubeHandleCap);
        if(amPressed) { ItemSelectionInterface.OpenWindow(); }

        GUI.DrawTexture(GUILayoutUtility.GetRect(15, 15), (Texture2D)Resources.Load("questionmark"), ScaleMode.ScaleToFit);
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
                string path = EditorUtility.OpenFilePanel("Load Level", "Assets/Levels", "asset");
                if (path.Length != 0)
                {
                    var fileContent = File.ReadAllBytes(path);
                    //texture.LoadImage(fileContent);
                    //CustomRoomWindow.OpenWindow(0, Vector2Int.zero, Vector2Int.zero);
                }
            }

            if (GUILayout.Button("Event Manager", GUILayout.Height(40), GUILayout.Width(200)))
            {
    //            EventManagerWindow.OpenWindow();
            }

            GUILayout.BeginArea(new Rect(0, 160, 200, 40));
            {
                if (GUILayout.Button("Close", GUILayout.Height(40), GUILayout.Width(200))) Close();
            }
            GUILayout.EndArea();
        }
         GUILayout.EndArea();
    }

    public void ScreenShot()
    {
        var width = 100;
        var height = 100;
        var startX = 100;
        var startY = 100;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rex = new Rect(startX, startY, width, height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        var bytes = tex.EncodeToPNG();
        DestroyImmediate(tex);

        System.IO.File.WriteAllBytes(Application.dataPath + "SavedScreen.png", bytes);
        //Modificar los valores para que tome una screencap del mapa y que el preview tenga el mismo nombre que el mapa guardado, ajustar el path para que sea el
        //mismo en donde se guardan los mapas


        //Debug.Log("screened");
    }

}

/*To do
  -Fix Grid selection sytem for HowTo menu and Preview
  -Add a screncap function (needs adjusting)
  -Fix the how to pop up window to the menu (Has a bug where moving the screen out of bounds and toggling it off makes it move to the center for one frame)
  -Add the preview system
  -Making it so it loads an already existing map
  */