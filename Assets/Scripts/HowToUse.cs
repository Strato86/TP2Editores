using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Object = UnityEngine.Object;


public class HowToUse : EditorWindow
{
    List<HowToNodes> howNodes;
    HowToNodes pickedNode;

    HowToCreate howCrea;
    Rect nodeGraph;

    int gridSep = 52;   

    public bool winOpened;

    public int selectId;

    public Color bgColor;

    public float popPosx;
    public float popPosy;

    public static void OpenWindow()
    {
        var self = (HowToUse)GetWindow(typeof(HowToUse), true, "How to use");
        self.position = new Rect(200, 100, 100, 200);
        self.nodeGraph = new Rect(5, 20, 90, 365);
        self.bgColor = new Color(0.5f, 0.55f, 0.6f, 1);
        self.pickedNode = new HowToNodes();
        self.howNodes = new List<HowToNodes>();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Help Menu");
        GUI.BeginGroup(nodeGraph);
        EditorGUI.DrawRect(new Rect(0, 0, 92, 367), Color.black);
        EditorGUI.DrawRect(new Rect(1, 1,88, 363), bgColor);   
 //       PaintGrid();
        popPosx = position.x + position.width;
        popPosy = position.y;

        /*        foreach (var Nod in howNodes)
                {
                    Nod.rect.width = nodeGraph.width;
                    Nod.rect.height = 40;
                    Nod.rect.y = Nod.gridY * gridSep;
                    Nod.color = Color.red;
                    Nod.identity += 1;
                    EditorGUI.DrawRect(Nod.rect, Nod.color);
                    Repaint();
                }*/
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
//            var fill = rHeight / 7;

//            var g = new HowToNodes(0, i * gridSep, rWidth, rHeight/7, 10, i,Color.red);
//            howNodes.Add(g);

//            EditorGUI.DrawRect(new Rect(0, i * gridSep, rWidth, fill), Color.gray);
            EditorGUI.DrawRect(new Rect(0, i * gridSep, rWidth, bord), Color.black);
        }
    }

    private void MouseInput(Event currentEvent)
    {
        if (!nodeGraph.Contains(currentEvent.mousePosition) || !(focusedWindow == this || mouseOverWindow == this)) return;

        if (currentEvent.button == 0 && currentEvent.type == EventType.MouseDown)
        {
            DrawPrefabNode(currentEvent);
        }

        HowToNodes selectNode = null;
        for (int i = 0; i < howNodes.Count; i++)
        {
            if (howNodes[i].overnode) selectNode = howNodes[i];
        }

        var prevSel = pickedNode;
        if (currentEvent.button == 0 && currentEvent.type == EventType.MouseDown)
        {
            if (selectNode != null) pickedNode = selectNode; 
            else pickedNode = null;
            pickedNode.SelectColorID(Color.blue,1);
            if (prevSel != pickedNode) Repaint();
        }

    }

    private void DrawPrefabNode(Event currentEvent)
    {
        var x = (int)((currentEvent.mousePosition.x - nodeGraph.x) / gridSep);
        var y = (int)((currentEvent.mousePosition.y - nodeGraph.y) / gridSep);

        HowToNodes drNode = new HowToNodes();
        var id = pickedNode.identity;
        bool isOcupied = false;

            if(!isOcupied)
            {
                var g = new HowToNodes(x, y * gridSep, nodeGraph.width, nodeGraph.height / 7, id , 7, Color.grey);
                drNode = g;
            }else
            {
                drNode.SelectColorID(pickedNode.color, id);
            }
    }


    private void Features()
    {
        maxSize = new Vector2(100, 400);
        minSize = new Vector2(100, 400);

        GUILayout.BeginArea(new Rect(10, 25, 88, 365));            
        if (GUILayout.Button("Starting", GUILayout.Height(40), GUILayout.Width(80)))
        {
            PopWin();
            selectId = 0;
        }
        if (GUILayout.Button("Basic tools", GUILayout.Height(40), GUILayout.Width(80))) { PopWin(); selectId = 1; };
        if (GUILayout.Button("Adv. tools", GUILayout.Height(40), GUILayout.Width(80))) { PopWin();  selectId = 2; };
        if (GUILayout.Button("Layers", GUILayout.Height(40), GUILayout.Width(80))) { PopWin(); selectId = 3; };
        if (GUILayout.Button("Events", GUILayout.Height(40), GUILayout.Width(80))) { PopWin(); selectId = 4; };
        if (GUILayout.Button("Enemies", GUILayout.Height(40), GUILayout.Width(80))) { PopWin(); selectId = 5; };
        if (GUILayout.Button("Website", GUILayout.Height(40), GUILayout.Width(80))) { PopWin(); selectId = 6; };
        GUILayout.EndArea();
    }

    private void PopWin()
    {
        if (!winOpened)
        {
            var w = ScriptableObject.CreateInstance<HowToCreate>();
            //w.position = new Rect(popPos, 100, 100, 500);
            w.howUse = this;
            //max 800 500
            w.ShowPopup();
            winOpened = true;
        }
    }
}
