using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;


public class HowToUse : EditorWindow
{
    HowToCreate howCrea;
    Rect nodeGraph;

    string[] Labeling = { "Starting", "Basic Tools", "Advanced", "Layers", "Events", "Enemies", "Website" };

    int gridSep = 52;

    public bool winOpened;

    public int selectId;
    public int hoverId;

    public Color bgColor;
    public Color selbgColor;
    private Color _textColor;
    public Color hovbgColor;

    public float popPosx;
    public float popPosy;

    private GUIStyle _textStyle;

    public void OnEnable()
    {
        _textColor = new Color(0.9f, 0.9f, 0.9f, 1);
        _textStyle = new GUIStyle();
        _textStyle.fontSize = 12;
        _textStyle.normal.textColor = _textColor;
        _textStyle.fontStyle = FontStyle.Bold;
        _textStyle.alignment = TextAnchor.MiddleCenter;
    }

    public static void OpenWindow()
    {
        var self = (HowToUse)GetWindow(typeof(HowToUse), true, "How to use");
        self.position = new Rect(200, 100, 100, 200);
        self.nodeGraph = new Rect(5, 20, 90, 365);
        self.bgColor = new Color(0.5f, 0.55f, 0.6f, 1);
        self.selbgColor = new Color(0.3f, 0.55f, 0.9f, 0.5f);
        self.hovbgColor = new Color(1f, 1f, 1f, 0.5f);

    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Help Menu");
        GUI.BeginGroup(nodeGraph);
        EditorGUI.DrawRect(new Rect(0, 0, 92, 367), Color.black);
        PaintGrid();
        popPosx = position.x + position.width;
        popPosy = position.y;
        if (selectId != 0)
        {
            EditorGUI.DrawRect(new Rect(1, (selectId - 1) * gridSep + 1, nodeGraph.width - 2, nodeGraph.height / 7 - 1), selbgColor);
        }


        if (hoverId != 0)
        {
            EditorGUI.DrawRect(new Rect(1, (hoverId - 1) * gridSep + 1, nodeGraph.width - 2, nodeGraph.height / 7 - 1), hovbgColor);
            Repaint();
        }

        GUI.EndGroup();
        Features();
    }

    void PaintGrid()
    {
        for (int i = 0; i * gridSep + nodeGraph.y <= position.height; i++)
        {
            var bord = 1;
            var rHeight = nodeGraph.height;
            var rWidth = nodeGraph.width;
            var fill = rHeight / 7;

            Rect squarButton;
            squarButton = new Rect(1, i * gridSep, rWidth - 2, fill);
            EditorGUI.DrawRect(squarButton, bgColor);

            var marg2 = new Vector2(40, i * gridSep);
            if (squarButton.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown)
            {
                PopWin();
                selectId = i + 1;
                Repaint();
            }

            if (squarButton.Contains(Event.current.mousePosition))
            {
                hoverId = i + 1;
            }

            EditorGUI.DrawRect(new Rect(0, i * gridSep, rWidth, bord), Color.black);

            if (i < Labeling.Length)
                EditorGUI.LabelField(new Rect(0, i * gridSep, rWidth, fill), Labeling[i], _textStyle);
        }
    }



    private void Features()
    {
        maxSize = new Vector2(100, 400);
        minSize = new Vector2(100, 400);
    }

    private void PopWin()
    {
        if (!winOpened)
        {
            var w = ScriptableObject.CreateInstance<HowToCreate>();
            w.howUse = this;
            //max 800 500
            w.ShowPopup();
            winOpened = true;
        }
    }
}
