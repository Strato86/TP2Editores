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


    public static void OpenWindow()
    {
        var self = (ItemSelectionInterface)GetWindow(typeof(ItemSelectionInterface), true, "Preview");
        self.position = new Rect(120, 100, 250, 500);
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
        GUI.DrawTexture(GUILayoutUtility.GetRect(223, 223), (Texture2D)Resources.Load("HowTo/previewTest2"), ScaleMode.StretchToFill);
        GUILayout.EndArea();
        EditorGUI.DrawRect(new Rect(1, 1, 223, 223), new Color(0.7f, 0.7f, 0.7f, 0.5f));
        GUILayout.EndArea();

        //Dividing lines    
        EditorGUI.DrawRect(new Rect(0, 247, 500, 1), Color.gray);
        EditorGUI.DrawRect(new Rect(0, 248, 500, 1), Color.white);

        //Bottom square
        GUILayout.BeginArea(new Rect(12, 260, 225, 225));
        EditorGUI.DrawRect(new Rect(0, 0, 225, 225), Color.black);
        EditorGUI.DrawRect(new Rect(1, 1, 223, 223), new Color(0.6f, 0.6f, 0.6f, 1));
        GUILayout.BeginArea(new Rect(0, 0, 225, 225));

        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, false, false);
         PaintGrid();
         FileList();
        EditorGUILayout.EndScrollView();

        GUILayout.EndArea();

        GUILayout.EndArea();

        //Al guardarse habria que ver como guardar una screenshot del mapa en si asi lo cargamos aca como preview
    }

    void FileList()
    {
        //     foreach(Texture2D t in Resources.FindObjectsOfTypeAll(typeof(Texture2D)) as Texture2D[])
        var textures = Resources.LoadAll("HowTo", typeof(Texture2D));
        foreach (Texture2D t in textures)
        {
            previews.Add(t);
        }

        for (int i = 0; i < textures.Length; i++)
        {
            EditorGUILayout.LabelField(previews[i].name);
            //Debug.Log(i);  
        }
    }

    void PaintGrid()
    {     
        for (int i = 0; i * gridSep + prevList.y <= position.height; i++)
        {
            var bord = 1;
            var rHeight = prevList.height / 10;
            var rWidth = prevList.width;
            EditorGUI.DrawRect(new Rect(0, i * gridSep, rWidth, bord), Color.black);
        }        
    }

}
