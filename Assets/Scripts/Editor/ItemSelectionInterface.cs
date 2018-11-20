using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;


public class ItemSelectionInterface : EditorWindow
{
    List<Texture2D> previews = new List<Texture2D>();
    Texture2D selPreview;
    Rect prevList = new Rect(0, 0, 230, 225);
    int gridSep = 17;
    private Vector2 _scrollPosition;
    string loadName;
    int selLine;
    Color selColor;
    bool check;

    public static void OpenWindow()
    {
        var self = (ItemSelectionInterface)GetWindow(typeof(ItemSelectionInterface), true, "Preview");
        self.position = new Rect(120, 100, 250, 525);
        self.selColor = new Color(0, 0, 1, 0.5f);
        //self.crwRef = new CustomRoomWindow();
    }

    private void OnGUI()
    {
        Features();
    }

    private void Features()
    {
        //Preview square
        GUILayout.BeginArea(new Rect(12, 12, 225, 225));
        EditorGUI.DrawRect(new Rect(0, 0, 225, 225), Color.black);
        EditorGUI.DrawRect(new Rect(1, 1, 223, 223), Color.gray);
        GUILayout.BeginArea(new Rect(1, 1, 223, 223));
        if (selLine <= previews.Count && selLine > 0)
        {
            GUI.DrawTexture(GUILayoutUtility.GetRect(223, 223), (Texture2D)Resources.Load("Previews/" + previews[selLine - 1].name), ScaleMode.StretchToFill);
        }

        GUILayout.EndArea();
        EditorGUI.DrawRect(new Rect(1, 1, 223, 223), new Color(0.7f, 0.7f, 0.7f, 0.5f));
        GUILayout.EndArea();

        //Dividing lines    
        EditorGUI.DrawRect(new Rect(0, 247, 250, 1), Color.gray);
        EditorGUI.DrawRect(new Rect(0, 248, 250, 1), Color.white);

        EditorGUI.DrawRect(new Rect(0, 280, 250, 1), Color.gray);
        EditorGUI.DrawRect(new Rect(0, 281, 250, 1), Color.white);

        //middle Square
        //Side Square
        GUILayout.BeginArea(new Rect(2, 252, 500, 250));
        EditorGUILayout.LabelField("Load Room", EditorStyles.boldLabel);
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(7, 262, 500, 250));
        EditorGUILayout.LabelField("(Input name)", EditorStyles.miniLabel);
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(80, 257, 500, 250));
        loadName = EditorGUILayout.TextField(loadName, GUILayout.Width(110));
        GUILayout.EndArea();
        GUILayout.BeginArea(new Rect(200, 255, 500, 250));
        if (GUILayout.Button("Load", GUILayout.Height(20), GUILayout.Width(40)))
        {
            if (loadName != "")
            {
                CustomRoomWindow.OpenWindow(0, Vector2Int.zero, Vector2Int.zero);
            }
        }
        GUILayout.EndArea();

        //Bottom square
        GUILayout.BeginArea(new Rect(12, 290, 225, 225));
        EditorGUI.DrawRect(new Rect(0, 0, 225, 225), Color.black);
        //EditorGUI.DrawRect(new Rect(1, 1, 223, 223), new Color(0.6f, 0.6f, 0.6f, 1));
        GUILayout.BeginArea(new Rect(0, 0, 225, 225));
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
        PaintGrid();
        FileList();
        if (selLine != 0)
        {
            EditorGUI.DrawRect(new Rect(1, (selLine - 1) * gridSep + 1, 223, gridSep - 1), selColor);
        }
        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();
        GUILayout.EndArea();


    }

    void FileList()
    {
        var textures = Resources.LoadAll("Previews", typeof(Texture2D));
        if (!check)
        {
            foreach (Texture2D t in textures)
            {
                previews.Add(t);
            }
            check = true;
        }
        for (int i = 0; i < textures.Length; i++)
        {
            EditorGUILayout.LabelField(previews[i].name);
        }
    }

    void PaintGrid()
    {
        for (int i = 0; i * gridSep + prevList.y <= position.height; i++)
        {
            Rect squarButton;
            var bord = 1;
            var rWidth = prevList.width - 7;
            squarButton = new Rect(1, i * gridSep, rWidth, gridSep);
            var marg2 = new Vector2(0, i * gridSep);
            EditorGUI.DrawRect(squarButton, Color.gray);
            if (squarButton.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                selLine = i + 1;
                Repaint();
            }
            EditorGUI.DrawRect(new Rect(0, i * gridSep, rWidth + 2, bord), Color.black);
        }
    }


}
